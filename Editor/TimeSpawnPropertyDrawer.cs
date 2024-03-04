using System;
using _Project.Scripts.ThirdPartyNotifications;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(UTimeSpan))]
    public class TimeSpawnPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            var ticks = property.FindPropertyRelative("_ticks");
            
            var ticksValue = EditorGUI.LongField(new Rect(
                position.x, position.y,
                150, position.height
            ), ticks.longValue);
            
            ticks.serializedObject.Update();
            ticks.longValue = ticksValue;
            ticks.serializedObject.ApplyModifiedProperties();

            var ts = new TimeSpan(ticks.longValue);
            var days = ts.Days;
            var hours = ts.Hours;
            var minutes = ts.Minutes;
            var seconds = ts.Seconds;
            var milliseconds = ts.Milliseconds;

            ts = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            EditorGUI.LabelField(new Rect(
                position.x + 150, position.y,
                150, position.height
            ), ts.ToString());

            EditorGUI.EndProperty();
        }
    }
}