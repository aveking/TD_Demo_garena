﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK
{

    public enum _ShootObjectType { Projectile, Missile, Beam, Effect, FPSProjectile, FPSBeam, FPSEffect }

    public class ShootObject : MonoBehaviour
    {

        public _ShootObjectType type;

        public float speed = 5f; //子弹的速度，是通过MonoBehaviour进行的数据传入
        public float beamDuration = .5f;

        private Transform shootPoint;

        private bool hit = false;

        public bool autoSearchLineRenderer = true;
        public List<LineRenderer> lineList = new List<LineRenderer>();

        private List<TrailRenderer> trailList = new List<TrailRenderer>();


        public GameObject shootEffect;
        public bool destroyShootEffect = true;
        public float destroyShootDuration = 1.5f;

        public GameObject hitEffect;
        public bool destroyHitEffect = true;
        public float destroyHitDuration = 1.5f;

        public GameObject destroyEffect;
        public bool destroyDestroyEffect = true;
        public float destroyDestroyDuration = 1.5f;


        private AttackInstance attInstance;

        private GameObject thisObj;
        private Transform thisT;

        void Awake()
        {
            thisObj = gameObject;
            thisT = transform;

            thisObj.layer = TDTK.GetLayerShootObject();

            if (autoSearchLineRenderer)
            {
                LineRenderer[] lines = thisObj.GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++) lineList.Add(lines[i]);
            }

            TrailRenderer[] trails = thisObj.GetComponentsInChildren<TrailRenderer>(true);
            for (int i = 0; i < trails.Length; i++) trailList.Add(trails[i]);

            if (type == _ShootObjectType.FPSProjectile)
            {
                SphereCollider sphereCol = GetComponent<SphereCollider>();
                if (sphereCol == null)
                {
                    sphereCol = thisObj.AddComponent<SphereCollider>();
                    sphereCol.radius = 0.15f;
                }
                hitRadius = sphereCol.radius;
            }

            if (shootEffect != null) ObjectPoolManager.New(shootEffect);
            if (hitEffect != null) ObjectPoolManager.New(hitEffect);
            if (destroyEffect != null) ObjectPoolManager.New(destroyEffect);
        }

        void OnEnable()
        {
            //for(int i=0; i<trailList.Count; i++) StartCoroutine(ClearTrail(trailList[i]));
        }
        void OnDisable()
        {

        }

        public void Shoot(AttackInstance attInst = null, Transform sp = null)
        {
            if (attInst.tgtUnit == null || attInst.tgtUnit.GetTargetT() == null)
            {
                ObjectPoolManager.Unspawn(thisObj);
                return;
            }

            attInstance = attInst;
            target = attInstance.tgtUnit;
            targetPos = target.GetTargetT().position;
            hitThreshold = Mathf.Max(0.1f, target.hitThreshold);
            hitThreshold = 1.5f; //让子弹的碰撞离圆心远点

            shootPoint = sp;
            if (shootPoint != null) thisT.rotation = shootPoint.rotation;

            ShootEffect();

            hit = false;

            if (type == _ShootObjectType.Projectile) StartCoroutine(ProjectileRoutine());
            if (type == _ShootObjectType.Beam) StartCoroutine(BeamRoutine());
            if (type == _ShootObjectType.Missile) StartCoroutine(MissileRoutine());
            if (type == _ShootObjectType.Effect) StartCoroutine(EffectRoutine());

            //Debug.Log($"shoot type={type} speed={speed}");
            //speed = 1f;
        }

        public void ShootFPS(AttackInstance attInst = null, Transform sp = null)
        {
            shootPoint = sp;
            if (shootPoint != null) thisT.rotation = shootPoint.rotation;

            ShootEffect();

            hit = false;
            attInstance = attInst;
            if (type == _ShootObjectType.FPSProjectile) StartCoroutine(FPSProjectileRoutine());
            if (type == _ShootObjectType.FPSBeam) StartCoroutine(FPSBeamRoutine(sp));
            if (type == _ShootObjectType.FPSEffect) StartCoroutine(FPSEffectRoutine());
        }


        public float hitRadius = .1f;
        IEnumerator FPSEffectRoutine()
        {
            yield return new WaitForSeconds(0.05f);

            RaycastHit raycastHit;
            Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
            if (Physics.SphereCast(thisT.position, hitRadius / 2, dir, out raycastHit))
            {
                Unit unit = raycastHit.transform.GetComponent<Unit>();
                FPSHit(unit, raycastHit.point);

                HitEffect(raycastHit.point);
            }

            yield return new WaitForSeconds(0.1f);
            ObjectPoolManager.Unspawn(thisObj);
        }


        IEnumerator FPSBeamRoutine(Transform sp)
        {
            thisT.parent = sp;
            float duration = 0;
            while (duration < beamDuration)
            {
                RaycastHit raycastHit;
                Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
                bool hitCollider = Physics.SphereCast(thisT.position, hitRadius, dir, out raycastHit);
                if (hitCollider)
                {
                    if (!hit)
                    {
                        hit = true;
                        Unit unit = raycastHit.transform.GetComponent<Unit>();
                        FPSHit(unit, raycastHit.point);

                        HitEffect(raycastHit.point);
                    }
                }

                float lineDist = raycastHit.distance == 0 ? 9999 : raycastHit.distance;
                for (int i = 0; i < lineList.Count; i++) lineList[i].SetPosition(1, new Vector3(0, 0, lineDist));

                duration += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            thisT.parent = null;
            ObjectPoolManager.Unspawn(thisObj);
        }


        IEnumerator FPSProjectileRoutine()
        {
            float timeShot = Time.time;
            while (true)
            {
                RaycastHit raycastHit;
                Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
                float travelDist = speed * Time.fixedDeltaTime;
                bool hitCollider = Physics.SphereCast(thisT.position, hitRadius, dir, out raycastHit, travelDist);
                if (hitCollider) travelDist = raycastHit.distance + hitRadius;

                thisT.Translate(Vector3.forward * travelDist);
                if (Time.time - timeShot > 5) break;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            ObjectPoolManager.Unspawn(thisObj);
            yield return null;
        }
        void OnTriggerEnter(Collider collider)
        {
            //enable this to prevent non-fps type to shoot through collider layered as terrain
            //if(collider.gameObject.layer==TDTK.GetLayerTerrain()){
            //	HitEffect(thisT.position);
            //	ObjectPoolManager.Unspawn(thisObj);
            //	return;
            //}

            if (!hit && type != _ShootObjectType.FPSProjectile) return;

            hit = true;

            attInstance.impactPoint = thisT.position;

            Unit unit = collider.gameObject.GetComponent<Unit>();
            FPSHit(unit, thisT.position);

            HitEffect(thisT.position);

            ObjectPoolManager.Unspawn(thisObj);
        }


        void FPSHit(Unit hitUnit, Vector3 hitPoint)
        {
            if (attInstance.srcWeapon.GetAOERange() > 0)
            {
                LayerMask mask1 = 1 << TDTK.GetLayerCreep();
                LayerMask mask2 = 1 << TDTK.GetLayerCreepF();
                LayerMask mask = mask1 | mask2;

                Collider[] cols = Physics.OverlapSphere(hitPoint, attInstance.srcWeapon.GetAOERange(), mask);
                if (cols.Length > 0)
                {
                    List<Unit> tgtList = new List<Unit>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit unit = cols[i].gameObject.GetComponent<Unit>();
                        if (!unit.IsDestroyed()) tgtList.Add(unit);
                    }
                    if (tgtList.Count > 0)
                    {
                        for (int i = 0; i < tgtList.Count; i++)
                        {
                            AttackInstance attInst = new AttackInstance();
                            attInst.srcWeapon = attInstance.srcWeapon;
                            attInst.tgtUnit = tgtList[i];
                            attInst.Process();
                            tgtList[i].ApplyEffect(attInst);
                        }
                    }
                }
            }
            else
            {
                if (hitUnit != null && hitUnit.IsCreep())
                {
                    attInstance.tgtUnit = hitUnit;
                    attInstance.Process();
                    hitUnit.ApplyEffect(attInstance);
                }
            }
        }









        private Unit target;
        private Vector3 targetPos;
        public float maxShootAngle = 30f;
        public float maxShootRange = 0.5f;
        private float hitThreshold = 8.15f;


        public float GetMaxShootRange()
        {
            if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile) return maxShootRange;
            return 1;
        }
        public float GetMaxShootAngle()
        {
            if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile) return maxShootAngle;
            return 0;
        }


        IEnumerator EffectRoutine()
        {
            yield return new WaitForSeconds(0.125f);
            Hit();
        }

        IEnumerator BeamRoutine()
        {
            float timeShot = Time.time;
            Vector3 shootP = shootPoint.position;

            while (!hit)
            {
                if (target != null) targetPos = target.GetTargetT().position;
                if (shootPoint != null) shootP = shootPoint.position;

                float dist = Vector3.Distance(shootP, targetPos);
                Ray ray = new Ray(shootP, (targetPos - shootP));
                Vector3 targetPosition = ray.GetPoint(dist - hitThreshold);

                for (int i = 0; i < lineList.Count; i++)
                {
                    lineList[i].SetPosition(0, shootP);
                    lineList[i].SetPosition(1, targetPosition);
                }

                if (Time.time - timeShot > beamDuration)
                {
                    Hit();
                    break;
                }

                yield return null;
            }
        }


        IEnumerator ProjectileRoutine() //炮弹，模拟程序
        {
            magic_book.Push_One(thisT);
            ShootEffect();

            float timeShot = Time.time;

            //make sure the shootObject is facing the target and adjust the projectile angle
            thisT.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(thisT.position, targetPos) / maxShootRange) * maxShootAngle;
            //clamp the angle magnitude to be less than 45 or less the dist ratio will be off
            thisT.rotation = thisT.rotation * Quaternion.Euler(-angle, 0, 0);

            Vector3 startPos = thisT.position;
            float iniRotX = thisT.rotation.eulerAngles.x;

            float y = Mathf.Min(targetPos.y, startPos.y);
            float totalDist = Vector3.Distance(startPos, targetPos);

            //while the shootObject havent hit the target
            while (!hit)
            {
                if (thisT.gameObject.layer == 30)
                {
                    if (attInstance.destroy) DestroyEffect(targetPos);
                    global_gamesetting._inst?.PlayHitEffect(thisT.position);
                    thisT.gameObject.SetActive(false);
                    //ObjectPoolManager.Unspawn(thisObj);
                    break; //被魔法书阻挡掉了
                }
                while (hand_cards.card5_stop_cd > 0f) yield return null;


                if (target != null) targetPos = target.GetTargetT().position;

                //calculating distance to targetPos
                Vector3 curPos = thisT.position;
                curPos.y = y;
                float currentDist = Vector3.Distance(curPos, targetPos);
                float curDist = Vector3.Distance(thisT.position, targetPos);
                //if the target is close enough, trigger a hit
                if (curDist < hitThreshold && !hit)
                {
                    Hit();
                    break;
                }

                if (Time.time - timeShot < 3.5f)
                {
                    //calculate ratio of distance covered to total distance
                    float invR = 1 - Mathf.Min(1, currentDist / totalDist);

                    //use the distance information to set the rotation, 
                    //as the projectile approach target, it will aim straight at the target
                    Vector3 wantedDir = targetPos - thisT.position;
                    if (wantedDir != Vector3.zero)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(wantedDir);
                        float rotX = Mathf.LerpAngle(iniRotX, wantedRotation.eulerAngles.x, invR);

                        //make y-rotation always face target
                        thisT.rotation = Quaternion.Euler(rotX, wantedRotation.eulerAngles.y, wantedRotation.eulerAngles.z);
                    }
                }
                else
                {
                    //this shoot time exceed 3.5sec, abort the trajectory and just head to the target
                    thisT.LookAt(targetPos);
                }

                //move forward
                thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, curDist));
                //Debug.Log($"fix frame move shoot type={type} speed={speed}");



                curDist = Vector3.Distance(thisT.position, targetPos);
                if (curDist < hitThreshold && !hit) { Hit(); break; }

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            magic_book.Pop_One(thisT);
        }


        public float shootAngleY = 20;
        private float missileSpeedModifier = 1;
        IEnumerator MissileSpeedRoutine()
        {
            missileSpeedModifier = .05f;
            float duration = 0;
            while (duration < 1)
            {
                missileSpeedModifier = Mathf.Sin(Mathf.Sin(duration * Mathf.PI / 2) * Mathf.PI / 2);
                duration += Time.deltaTime * 1f;
                yield return null;
            }
            missileSpeedModifier = 1;
        }
        IEnumerator MissileRoutine()
        {
            StartCoroutine(MissileSpeedRoutine());

            float angleX = Random.Range(maxShootAngle / 2, maxShootAngle);
            float angleY = Random.Range(shootAngleY / 2, maxShootAngle);
            if (Random.Range(0f, 1f) > 0.5f) angleY *= -1;
            thisT.LookAt(targetPos);
            thisT.rotation = thisT.rotation;
            Quaternion wantedRotation = thisT.rotation * Quaternion.Euler(-angleX, angleY, 0);
            float rand = Random.Range(4f, 10f);

            float totalDist = Vector3.Distance(thisT.position, targetPos);

            float estimateTime = totalDist / speed;
            float shootTime = Time.time;

            Vector3 startPos = thisT.position;

            while (!hit)
            { //显示数学上的轨迹跟踪
                if (target != null) targetPos = target.GetTargetT().position;
                float currentDist = Vector3.Distance(thisT.position, targetPos);

                float delta = totalDist - Vector3.Distance(startPos, targetPos);
                float eTime = estimateTime - delta / speed;

                if (Time.time - shootTime > eTime)
                {
                    Vector3 wantedDir = targetPos - thisT.position;
                    if (wantedDir != Vector3.zero)
                    {
                        wantedRotation = Quaternion.LookRotation(wantedDir);
                        float val1 = (Time.time - shootTime) - (eTime);
                        thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, val1 / (eTime * currentDist));
                    }
                }
                else thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, Time.fixedDeltaTime * rand);

                if (currentDist < hitThreshold)
                {
                    Hit();
                    break;
                }

                thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.fixedDeltaTime * missileSpeedModifier, currentDist));

                currentDist = Vector3.Distance(thisT.position, targetPos);
                if (currentDist < hitThreshold && !hit) { Hit(); break; }

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }





        void Hit()
        {
            hit = true;

            HitEffect(targetPos);

            //thisT.position=targetPos;
            attInstance.impactPoint = targetPos;

            if (attInstance.srcUnit.GetAOERadius() > 0)
            {
                LayerMask mask = attInstance.srcUnit.GetTargetMask();

                Collider[] cols = Physics.OverlapSphere(targetPos, attInstance.srcUnit.GetAOERadius(), mask);
                if (cols.Length > 0)
                {
                    List<Unit> tgtList = new List<Unit>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit unit = cols[i].gameObject.GetComponent<Unit>();
                        if (!unit.IsDestroyed()) tgtList.Add(unit);
                    }
                    if (tgtList.Count > 0)
                    {
                        for (int i = 0; i < tgtList.Count; i++)
                        {
                            if (tgtList[i] == target)
                            {
                                target.ApplyEffect(attInstance);
                                if (attInstance.destroy) DestroyEffect(targetPos);
                                //global_gamesetting._inst?.PlayHitEffect(thisT.position);
                            }
                            else
                            {
                                AttackInstance attInst = new AttackInstance();
                                attInst.srcUnit = attInstance.srcUnit;
                                attInst.tgtUnit = tgtList[i];
                                attInst.Process();
                                tgtList[i].ApplyEffect(attInst);

                                if (attInst.destroy) DestroyEffect(tgtList[i].thisT.position);
                                //global_gamesetting._inst?.PlayHitEffect(thisT.position);
                            }
                        }
                    }
                }
            }
            else
            {
                if (target != null) target.ApplyEffect(attInstance);

                if (attInstance.destroy) DestroyEffect(targetPos);
            }

            ObjectPoolManager.Unspawn(thisObj);
        }



        private void ShootEffect()
        {
            if (shootEffect != null)
            {
                if (!destroyShootEffect) ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation);
                else ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation, destroyShootDuration);
            }
        }
        private void HitEffect(Vector3 tgtPos)
        {
            if (hitEffect != null)
            {
                if (!destroyHitEffect) ObjectPoolManager.Spawn(hitEffect, tgtPos, thisT.rotation);
                else ObjectPoolManager.Spawn(hitEffect, tgtPos, thisT.rotation, destroyHitDuration);
            }
        }
        private void DestroyEffect(Vector3 tgtPos)
        {
            if (destroyEffect != null)
            {
                if (!destroyDestroyEffect) ObjectPoolManager.Spawn(destroyEffect, tgtPos, thisT.rotation);
                else ObjectPoolManager.Spawn(destroyEffect, tgtPos, thisT.rotation, destroyDestroyDuration);
            }
        }





        IEnumerator ClearTrail(TrailRenderer trail)
        {
            if (trail == null) yield break;
            float trailDuration = trail.time;
            trail.time = -1;
            yield return null;
            trail.time = trailDuration;
        }



    }

}