using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Car))]
public class CarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Car car = (Car)target;

        car.prefab = (GameObject)EditorGUILayout.ObjectField(car.prefab, typeof(GameObject), true);

        car.spawnPos = EditorGUILayout.Vector3Field("Posição Spawn", car.spawnPos);
        car.speed = EditorGUILayout.IntField("Velocidade Atual", car.speed);
        car.gear = EditorGUILayout.IntField("Marcha", car.gear);

        EditorGUILayout.LabelField("Velocidade Máxima", car.totalSpeed.ToString());

        EditorGUILayout.HelpBox("Cálculo de Velocidade Máxima", MessageType.Info);

        if(car.totalSpeed > 150)
        {
            EditorGUILayout.HelpBox("Overheat", MessageType.Error);
        }

        GUI.color = Color.yellow;
        if(GUILayout.Button("Spawn Prefab"))
        {
            car.Spawn();
        }
    }
}
