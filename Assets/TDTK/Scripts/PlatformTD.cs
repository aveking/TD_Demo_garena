using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK
{
    //path on platform
    public class SubPath
    {
        public delegate void PathChangedHandler(SubPath subPath);
        public static event PathChangedHandler onPathChangedE;

        public PathTD parentPath;
        public PlatformTD parentPlatform;
        public int wpIDPlatform = 0;

        public Transform connectStart;
        public Transform connectEnd;

        public NodeTD startN;
        public NodeTD endN;

        public List<Vector3> path = new List<Vector3>();
        public List<Vector3> altPath = new List<Vector3>(); //for checking if there's any block

        public void Init(PlatformTD platform)
        {
            parentPlatform = platform;

            startN = PathFinder.GetNearestNode(connectStart.position, platform.GetNodeGraph());
            endN = PathFinder.GetNearestNode(connectEnd.position, platform.GetNodeGraph());

            path.Add((connectStart.position + connectEnd.position) / 2);

            SearchNewPath(platform.GetNodeGraph());
        }

        public bool IsNodeInPath(NodeTD node)
        {
            float gridSize = BuildManager.GetGridSize();
            for (int i = 0; i < path.Count; i++)
            {
                float dist = Vector3.Distance(node.pos, path[i]);
                if (dist < gridSize * .85f) return true;
            }
            return false;
        }

        public void SearchNewPath(NodeTD[] nodeGraph)
        {
            PathFinder.GetPath(startN, endN, nodeGraph, this.SetPath);
        }

        public void SetPath(List<Vector3> wpList)
        {
            path = wpList;
            if (onPathChangedE != null) onPathChangedE(this);
        }

        public void SwitchToSubPath()
        {
            SetPath(PathFinder.SmoothPath(altPath));
            altPath = new List<Vector3>();
        }
    }





    public class PlatformTD : MonoBehaviour
    {

        public Vector2 size;    //the grid-size of the platform (x*y)

        //prefabID of tower available to this platform
        //prior to runtime, this stores the ID of all the unavailable tower on the list, it gets reverse in VerifyTowers (call by BuildManager)
        public List<int> unavailableTowerIDList = new List<int>();
        [HideInInspector] public List<int> availableTowerIDList = new List<int>();  //only used in runtime

        //all the path crossing this platform
        public List<SubPath> subPathList = new List<SubPath>();

        //indicate if creep can walk pass this platform, true if this platform is a waypoint
        private bool walkable;

        [HideInInspector] public GameObject thisObj;
        [HideInInspector] public Transform thisT;

        static int tower_idx_build = 4;
        int tower_idx_my = 0;
        int tower_build_wait_frame = 0;
        public void Init(float gridSize, bool autoAdjustTextureToGrid, List<UnitTower> towerList)
        {
            thisObj = gameObject;
            thisT = transform;
            thisObj.layer = TDTK.GetLayerPlatform();

            Format(gridSize, autoAdjustTextureToGrid);

            VerifyTowers(towerList);

            if (!walkable) return;

            //Utility.DestroyColliderRecursively(thisT);

            GenerateNode();

            for (int i = 0; i < subPathList.Count; i++)
            {
                subPathList[i].Init(this);
            }
        }

        private void Start()
        {
            tower_idx_my = tower_idx_build++;
        }

        private void Update() //�Զ�����
        {
            if (tower_build_wait_frame < tower_idx_my)
            {
                ++tower_build_wait_frame;
                if (tower_build_wait_frame == tower_idx_my)
                {
                    BuildInfo buildInfo = new BuildInfo();
                    buildInfo.position = transform.position;
                    buildInfo.platform = this;
                    buildInfo.availableTowerIDList = availableTowerIDList;

                    BuildManager.BuildTower(3, buildInfo);

                    //BuildTower(transform.position, BuildManager.GetInstance().towerList[0]);
                    //Debug.Log($"husunren log auto build tower  tower_build_wait_frame={tower_build_wait_frame}");
                }
            }
        }

        public void Format(float gridSize, bool autoAdjustTextureToGrid)
        {
            //make sure the plane is perfectly horizontal, rotation around the y-axis is presreved
            thisT.eulerAngles = new Vector3(0, thisT.rotation.eulerAngles.y, 0);

            //adjusting the scale
            float scaleX = Mathf.Max(1, Mathf.Round(Utility.GetWorldScale(thisT).x / gridSize)) * gridSize;
            float scaleZ = Mathf.Max(1, Mathf.Round(Utility.GetWorldScale(thisT).z / gridSize)) * gridSize;

            thisT.localScale = new Vector3(scaleX, 1, scaleZ);

            size = new Vector2((int)(scaleX / gridSize), (int)(scaleZ / gridSize));

            //adjusting the texture
            if (autoAdjustTextureToGrid)
            {
                Material mat = thisT.GetComponent<Renderer>().material;

                float x = (Utility.GetWorldScale(thisT).x) / gridSize;
                float z = (Utility.GetWorldScale(thisT).z) / gridSize;

                mat.mainTextureOffset = new Vector2(0.5f, 0.5f);
                mat.mainTextureScale = new Vector2(x, z);
            }
        }


        public void VerifyTowers(List<UnitTower> towerList)
        {
            List<int> newList = new List<int>();
            for (int i = 0; i < towerList.Count; i++)
            {
                if (!unavailableTowerIDList.Contains(towerList[i].prefabID)) newList.Add(towerList[i].prefabID);
            }
            availableTowerIDList = newList;
        }


        public int AddSubPath(PathTD pathInstance, int wpID, Transform startP, Transform endP)
        {
            walkable = true;

            int ID = subPathList.Count;

            SubPath subPath = new SubPath();
            subPath.parentPath = pathInstance;
            subPath.wpIDPlatform = wpID;
            subPath.connectStart = startP;
            subPath.connectEnd = endP;

            subPathList.Add(subPath);

            return ID;
        }

        public List<Vector3> GetSubPathPath(int ID)
        {
            return subPathList[ID].path;
        }
        public SubPath GetSubPath(int ID)
        {
            return subPathList[ID];
        }



        public void BuildTower(Vector3 pos, UnitTower tower)
        {
            Debug.Log($"where we build one tower on platform TD");
            //pathfinding related code, only call if this platform is walkable;
            if (!walkable) return;

            if (tower.type != _TowerType.Mine)
            {
                NodeTD node = PathFinder.GetNearestNode(pos, nodeGraph);
                node.walkable = false;
                tower.SetPlatform(this, node);

                //if the node has been check before during CheckForBlock(), just use the altPath
                if (node == nextBuildNode)
                {
                    for (int i = 0; i < subPathList.Count; i++)
                    {
                        if (subPathList[i].IsNodeInPath(node)) subPathList[i].SwitchToSubPath();
                    }
                    return;
                }

                for (int i = 0; i < subPathList.Count; i++)
                {
                    if (subPathList[i].IsNodeInPath(node)) subPathList[i].SearchNewPath(nodeGraph);
                }

            }
        }
        public void UnbuildTower(NodeTD node)
        {
            node.walkable = true;
            for (int i = 0; i < subPathList.Count; i++)
            {
                subPathList[i].SearchNewPath(nodeGraph);
            }
        }


        //the node that will be blocked and the alt path for the current selected build pos, cache it so there's no need to do another search
        private NodeTD nextBuildNode;
        public bool CheckForBlock(Vector3 pos)
        {
            float gridSize = BuildManager.GetGridSize();
            NodeTD targetNode = PathFinder.GetNearestNode(pos, nodeGraph);

            for (int i = 0; i < subPathList.Count; i++)
            {
                SubPath subPath = subPathList[i];
                if (Vector3.Distance(pos, subPath.startN.pos) < gridSize / 2) return true;
                if (Vector3.Distance(pos, subPath.endN.pos) < gridSize / 2) return true;

                if (subPath.IsNodeInPath(targetNode))
                {
                    subPath.altPath = PathFinder.ForceSearch(subPath.startN, subPath.endN, targetNode, nodeGraph);
                    if (subPath.altPath.Count == 0) return true;
                }
            }

            nextBuildNode = targetNode;

            return false;
        }





        //set to true if creep can move pass this platform
        public void SetWalkable(bool flag)
        {
            walkable = flag;
        }

        public bool IsWalkable()
        {
            return walkable;
        }

        //the graph-node covering this platform
        private NodeTD[] nodeGraph;
        public NodeTD[] GetNodeGraph() { return nodeGraph; }
        public void GenerateNode(float heightOffset = 0) { nodeGraph = NodeGenerator.GenerateNode(this, 0); }

        public NodeTD GetNearestNode(Vector3 point) { return PathFinder.GetNearestNode(point, nodeGraph); }





        public bool GizmoShowNodes = true;
        void OnDrawGizmos()
        {
            if (GizmoShowNodes)
            {
                if (nodeGraph != null && nodeGraph.Length > 0)
                {
                    foreach (NodeTD node in nodeGraph)
                    {
                        if (!node.walkable)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(node.pos, BuildManager.GetGridSize() * .15f);
                        }
                        else
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawSphere(node.pos, BuildManager.GetGridSize() * .15f);
                        }
                    }
                }
            }
        }


    }



}