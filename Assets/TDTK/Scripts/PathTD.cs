using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TDTK;

namespace TDTK
{
    //a special struct contains all the info for a waypoint
    public class WPSection
    {
        public Transform waypointT;

        public bool isPlatform = false;

        //followng memebers are only used when the section is on platform
        public PlatformTD platform;
        public int pathIDOnPlatform = 0;    //in case there are more than 1 path crossing the platform, 
                                            //this ID will be used to identity which sub-path on the platform belong to which path
                                            //public bool subPathFound=false;	//indicate if the subpath on the platform has been found
    }

    //Boss和Minion要走的路径
    public class PathTD : MonoBehaviour
    {
        [HideInInspector] private bool isLinearPath = true;
        public bool IsLinearPath() { return isLinearPath; }

        public List<Transform> wpList = new List<Transform>();
        public List<WPSection> wpSectionList = new List<WPSection>();   //construct from wpList, each waypoint has each own WPSection

        public bool createPathLine = true;

        public float dynamicOffset = 1;

        public bool loop = false;
        public int loopPoint = 0;

        public int map_h = 100;
        public GameObject Platform_TD;//自动建造的炮塔
        public GameObject SceneTree;//场景中的树
        public int tower_cnt = 5;
        int map_w = 5;
        int[,] mapnode;
        const float NODE_SIZE = 2f;

        public void Init()
        {
            mapnode = new int[map_w, map_h];
            int pos_w = map_w >> 1;
            int branch_L = 30;
            int branch_R = branch_L + 30;
            int branch_Zero = branch_R + 40;
            int last_branch = 0; //防止出现锐角
            for (int i = 0; i < map_h; ++i)
            {
                if (i < 4) pos_w = map_w >> 1;
                mapnode[pos_w, i] = 1;
                int rnd = Random.Range(0, branch_Zero);
                if (rnd < branch_L)
                {
                    pos_w = pos_w >= map_w - 1 || last_branch == -1 ? pos_w : pos_w + 1;
                    branch_L = 60;
                    branch_R = branch_L + 30;
                    branch_Zero = branch_R + 40;
                    last_branch = 1;
                }
                else if (rnd < branch_R)
                {
                    pos_w = pos_w <= 0 || last_branch == 1 ? pos_w : pos_w - 1;
                    branch_L = 30;
                    branch_R = branch_L + 60;
                    branch_Zero = branch_R + 40;
                    last_branch = -1;
                }
                else
                {
                    branch_L = 30;
                    branch_R = branch_L + 30;
                    branch_Zero = branch_R + 60;
                    last_branch = 0;
                }
            }

            Transform start = wpList[0];
            Transform one = wpList[1];
            int last_end_idx = wpList.Count - 1;
            Transform end = wpList[last_end_idx];
            Transform parent = start.parent;
            for (int i = map_h - wpList.Count; i > 0; --i)
            {
                Transform ai = GameObject.Instantiate(one).transform;
                ai.SetParent(parent);
                wpList.Add(ai);
            }
            wpList[last_end_idx] = wpList[wpList.Count - 1];
            wpList[wpList.Count - 1] = end;

            //Debug.LogError($"wpList.Count={wpList.Count}");

            int max_rnd = 512;
            int add_step = 64;
            int check_rnd = max_rnd;
            for (int i = 0; i < map_h; ++i)
            {
                int lastwp_j = 0;
                for (int j = 0; j < map_w; ++j)
                {
                    if (mapnode[j, i] == 1)
                    {
                        wpList[i].position = new Vector3(i * NODE_SIZE, 0f, (j - (map_w >> 1)) * NODE_SIZE);
                        lastwp_j = j;
                    }
                }
                for (int j = 0; j < map_w; ++j)
                {
                    if (mapnode[j, i] != 1)
                    {
                        if (Mathf.Abs(lastwp_j - j) <= 2)//附近的可以放炮台
                        {
                            //int rnd = Random.Range(0, 1000);
                            //if (i >= 7 && i < map_h - 7 && rnd < check_rnd)
                            //{
                            //    Transform ai = GameObject.Instantiate(Platform_TD).transform;
                            //    ai.position = new Vector3(i * NODE_SIZE, 0f, (j - (map_w >> 1)) * NODE_SIZE);
                            //    check_rnd = 0;
                            //}
                            //else
                            //{
                            //    if (check_rnd < max_rnd) check_rnd += add_step;
                            //}

                            int rnd = Random.Range(0, 1000);
                            if (rnd < 64)
                            {
                                Transform ai = GameObject.Instantiate(SceneTree).transform;
                                ai.position = new Vector3(i * NODE_SIZE, Random.Range(-2f, -4f), (j - (map_w >> 1)) * NODE_SIZE);
                            }
                        }
                        else
                        {
                            int rnd = Random.Range(0, 1000);
                            if (i >= 7 && i < map_h - 7 && rnd < check_rnd)
                            {
                                Transform ai = GameObject.Instantiate(Platform_TD).transform;
                                ai.position = new Vector3(i * NODE_SIZE, 0f, (j - (map_w >> 1)) * NODE_SIZE);
                                check_rnd = 0;
                            }
                            else
                            {
                                if (check_rnd < max_rnd) check_rnd += add_step;

                                rnd = Random.Range(0, 1000);
                                if (rnd < 512)
                                {
                                    Transform ai = GameObject.Instantiate(SceneTree).transform;
                                    ai.position = new Vector3(i * NODE_SIZE, Random.Range(-2f, -4f), (j - (map_w >> 1)) * NODE_SIZE);
                                }
                            }




                        }
                    }

                }


            }


            wpSectionList = new List<WPSection>();
            for (int i = 0; i < wpList.Count; i++)
            {
                Transform wp = wpList[i];

                //check if this is a platform, BuildManager would have add the component and have them layered
                if (wpList[i] != null)
                {
                    WPSection section = new WPSection();
                    section.waypointT = wp;

                    if (wp.gameObject.layer == TDTK.GetLayerPlatform())
                    {
                        section.isPlatform = true;
                        section.platform = wp.gameObject.GetComponent<PlatformTD>();
                        section.pathIDOnPlatform = section.platform.AddSubPath(this, i, wpList[i - 1], wpList[i + 1]);

                        if (isLinearPath) isLinearPath = false;
                    }

                    wpSectionList.Add(section);
                }
                else
                {
                    wpList.RemoveAt(i);
                    i -= 1;
                }
            }

            if (loop)
            {
                loopPoint = Mathf.Min(wpList.Count - 1, loopPoint); //looping must be 1 waypoint before the destination
            }
        }


        public List<Vector3> GetWPSectionPath(int ID)
        {
            if (wpSectionList[ID].isPlatform)
            {
                WPSection section = wpSectionList[ID];
                return section.platform.GetSubPathPath(section.pathIDOnPlatform);
            }

            List<Vector3> list = new List<Vector3>();
            list.Add(wpSectionList[ID].waypointT.position);
            return list;
        }

        public int GetPathWPCount() { return wpList.Count; }
        public Transform GetSpawnPoint() { return wpList[0]; }

        public int GetLoopPoint() { return loopPoint; }


        public float GetPathDistance(int wpID = 1)
        {
            if (wpList.Count == 0) return 0;

            float totalDistance = 0;
            if (Application.isPlaying)
            {
                Vector3 lastPoint = wpList[0].position;
                for (int i = wpID; i < wpSectionList.Count; i++)
                {
                    if (wpSectionList[i].isPlatform)
                    {
                        List<Vector3> pointList = GetWPSectionPath(wpSectionList[i].pathIDOnPlatform);
                        totalDistance += Vector3.Distance(lastPoint, pointList[0]);
                        for (int n = 1; n < pointList.Count; n++)
                            totalDistance += Vector3.Distance(pointList[n - 1], pointList[n]);
                        lastPoint = pointList[pointList.Count - 1];
                    }
                    else
                    {
                        totalDistance += Vector3.Distance(lastPoint, wpSectionList[i].waypointT.position);
                        lastPoint = wpSectionList[i].waypointT.position;
                    }
                }
            }
            else
            {
                for (int i = wpID; i < wpList.Count; i++)
                    totalDistance += Vector3.Distance(wpList[i - 1].position, wpList[i].position);
            }
            return totalDistance;
        }





        void Start()
        {
            if (createPathLine) CreatePathLine();
        }
        void CreatePathLine()
        {

            Transform parentT = new GameObject().transform;
            parentT.position = transform.position;
            parentT.parent = transform;
            parentT.gameObject.name = "PathLine";

            GameObject pathLine = (GameObject)Resources.Load("ScenePrefab/PathLine");
            GameObject pathPoint = (GameObject)Resources.Load("ScenePrefab/PathPoint");

            Vector3 startPoint = Vector3.zero;
            Vector3 endPoint = Vector3.zero;

            SubPath subP = null;

            for (int i = 0; i < wpSectionList.Count; i++)
            {
                WPSection wpSec = wpSectionList[i];
                if (!wpSec.isPlatform)
                {
                    GameObject pointObj = (GameObject)Instantiate(pathPoint, wpSec.waypointT.position, Quaternion.identity);
                    endPoint = wpSec.waypointT.position;
                    pointObj.transform.parent = parentT;
                }
                else
                {
                    subP = wpSec.platform.GetSubPath(wpSec.pathIDOnPlatform);
                    GameObject point1Obj = (GameObject)Instantiate(pathPoint, subP.startN.pos, Quaternion.identity);
                    GameObject point2Obj = (GameObject)Instantiate(pathPoint, subP.endN.pos, Quaternion.identity);
                    endPoint = subP.startN.pos;

                    point1Obj.transform.parent = parentT;
                    point2Obj.transform.parent = parentT;
                }

                if (i > 0)
                {
                    GameObject lineObj = (GameObject)Instantiate(pathLine, startPoint, Quaternion.identity);
                    LineRenderer lineRen = lineObj.GetComponent<LineRenderer>();
                    lineRen.SetPosition(0, startPoint);
                    lineRen.SetPosition(1, endPoint);

                    lineObj.transform.parent = parentT;
                }

                if (wpSec.isPlatform) startPoint = subP.endN.pos;
                else startPoint = wpSec.waypointT.position;
            }
        }





        public bool showGizmo = true;
        public Color gizmoColor = Color.blue;
        void OnDrawGizmos()
        {
            if (showGizmo)
            {
                Gizmos.color = gizmoColor;

                if (Application.isPlaying)
                {
                    for (int i = 1; i < wpSectionList.Count; i++)
                    {
                        List<Vector3> subPathO = GetWPSectionPath(i - 1);
                        List<Vector3> subPath = GetWPSectionPath(i);

                        //Debug.Log(i+"    "+wpSectionList[i].isPlatform+"    "+subPathO.Count+"   "+subPath.Count);

                        Gizmos.DrawLine(subPathO[subPathO.Count - 1], subPath[0]);
                        for (int n = 1; n < subPath.Count; n++)
                        {
                            Gizmos.DrawLine(subPath[n - 1], subPath[n]);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < wpList.Count; i++)
                    {
                        if (wpList[i - 1] != null && wpList[i] != null)
                            Gizmos.DrawLine(wpList[i - 1].position, wpList[i].position);
                    }
                }
            }
        }

    }

}



