﻿using UnityEngine;

namespace ETModel
{
    public static class BulletFactory
    {
        public static Bullet Create(Tank tank)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            GameObject prefab = bundleGameObject.Get<GameObject>("Bullet");

            BulletComponent bulletComponent = tank.GetComponent<BulletComponent>();


            Bullet bullet = ComponentFactory.CreateWithId<Bullet, Tank>(IdGenerater.GenerateId(), tank);

            bullet.GameObject = UnityEngine.Object.Instantiate(prefab);

            GameObject parent = tank.GameObject.FindChildObjectByPath("bullets");

            bullet.GameObject.transform.SetParent(parent.transform,false);

            bulletComponent.Add(bullet);

            // 子弹添加飞行
            bullet.AddComponent<BulletFlyComponent>();


            return bullet;
            
        }
    }
}