﻿using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class TankMoveComponentUpdateSystem : UpdateSystem<TankMoveComponent>
    {
        public override void Update(TankMoveComponent self)
        {
            self.Update();
        }
    }

    public class TankMoveComponent : Component
    {
        public Vector3 Target;

        // 开启移动协程的时间
        public long StartTime;

        // 开启移动协程的Unit的位置
        public Vector3 StartPos;

        public long needTime;

        public ETTaskCompletionSource moveTcs;

        private float steer = 20;

        private float speed = 1;

        public void Update()
        {

            Transform transform = this.GetParent<Tank>().GameObject.transform;

            //旋转
            float x = Input.GetAxis("Horizontal");

            transform.Rotate(0,x*this.steer*Time.deltaTime,0);

            //前进和后退
            float y = Input.GetAxis("Vertical");

            Vector3 s = y * transform.forward * speed * Time.deltaTime;

            transform.position += s;

            return;

            // 上
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position += transform.forward * speed;
            }
            //下
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.eulerAngles = new Vector3(0,180,0);
                transform.position += transform.forward * speed;
            }
            //左
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.eulerAngles = new Vector3(0, 270, 0);
                transform.position += transform.forward * speed;
            }
            //右
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
                transform.position += transform.forward * speed;
            }



            return;
            if (this.moveTcs == null)
            {
                return;
            }

            Unit unit = this.GetParent<Unit>();
            long timeNow = TimeHelper.Now();

            if (timeNow - this.StartTime >= this.needTime)
            {
                unit.Position = this.Target;
                ETTaskCompletionSource tcs = this.moveTcs;
                this.moveTcs = null;
                tcs.SetResult();

                return;
            }

            float amount = (timeNow - this.StartTime) * 1f / this.needTime;
            unit.Position = Vector3.Lerp(this.StartPos, this.Target, amount);
        }

        public ETTask MoveToAsync(Vector3 target, float speedValue, CancellationToken cancellationToken)
        {
            Unit unit = this.GetParent<Unit>();

            if ((target - this.Target).magnitude < 0.1f)
            {
                return ETTask.CompletedTask;
            }

            this.Target = target;


            this.StartPos = unit.Position;
            this.StartTime = TimeHelper.Now();
            float distance = (this.Target - this.StartPos).magnitude;
            if (Math.Abs(distance) < 0.1f)
            {
                return ETTask.CompletedTask;
            }

            this.needTime = (long)(distance / speedValue * 1000);

            this.moveTcs = new ETTaskCompletionSource();

            cancellationToken.Register(() =>
            {
                this.moveTcs = null;
            });
            return this.moveTcs.Task;
        }
    }
}