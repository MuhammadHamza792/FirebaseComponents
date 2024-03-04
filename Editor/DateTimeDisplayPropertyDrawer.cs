using System;
using _Project.Scripts.ThirdPartyNotifications;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(DateTimeData))]
    
    public class DateTimeDisplayPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 50.0f;
        }
        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, label);
            var years = property.FindPropertyRelative("Years");
            var months = property.FindPropertyRelative("Months");
            var days = property.FindPropertyRelative("Days");
            var hours = property.FindPropertyRelative("Hours");
            var minutes = property.FindPropertyRelative("Minutes");
            var seconds = property.FindPropertyRelative("Seconds");
            var milliseconds = property.FindPropertyRelative("Milliseconds");
            
            
            EditorGUI.LabelField(new Rect(
                position.x + 130, position.y,
                60, 20
            ), "Years");
            
            var year = EditorGUI.IntField(new Rect(
                position.x + 170, position.y,
                50, 20 
            ), years.intValue);
            
            years.serializedObject.Update();
            years.intValue = year;
            years.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x + 40, position.y,
                60, 20
            ), "Months");
            
            var month = EditorGUI.IntField(new Rect(
                position.x + 90, position.y,
                50, 20
            ), months.intValue);
            
            months.serializedObject.Update();
            months.intValue = month;
            months.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x - 35f, position.y,
                150, 20
            ), "Days");
            
            var day = EditorGUI.IntField(new Rect(
                position.x, position.y,
                50, 20
            ), days.intValue);
            
            days.serializedObject.Update();
            days.intValue = day;
            days.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x - 40, position.y + 25,
                60, 20
            ), "Hours");
            
            var hour = EditorGUI.IntField(new Rect(
                position.x, position.y + 25,
                50, 20
            ), hours.intValue);
            
            hours.serializedObject.Update();
            hours.intValue = hour;
            hours.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x + 40, position.y + 25,
                60, 20
            ), "Mins");
            
            var mins = EditorGUI.IntField(new Rect(
                position.x + 73.5f, position.y + 25,
                50, 20
            ), minutes.intValue);
            
            minutes.serializedObject.Update();
            minutes.intValue = mins;
            minutes.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x + 115, position.y + 25,
                150, 20
            ), "Secs");
            
            var secs = EditorGUI.IntField(new Rect(
                position.x + 150, position.y + 25,
                50, 20
            ), seconds.intValue);
            
            seconds.serializedObject.Update();
            seconds.intValue = secs;
            seconds.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.LabelField(new Rect(
                position.x + 190, position.y + 25,
                150, 20
            ), "MSecs");
            
            var mSecs = EditorGUI.IntField(new Rect(
                position.x + 235, position.y + 25,
                50, 20
            ), milliseconds.intValue);
            
            milliseconds.serializedObject.Update();
            milliseconds.intValue = mSecs;
            milliseconds.serializedObject.ApplyModifiedProperties();

            /*var ts = new DateTime(ticks.longValue);
            var year = ts.Year;
            var month = ts.Month;
            var day = ts.Day;
            var hour = ts.Hour;
            var minute = ts.Minute;
            var second = ts.Second;
            var millisecond = ts.Millisecond;

            ts = new DateTime( year,month, day, hour, minute, second, millisecond);
            EditorGUI.LabelField(new Rect(
                position.x + 150, position.y,
                150, 20
            ), ts.T());*/

            EditorGUI.EndProperty();
        }
    }
}