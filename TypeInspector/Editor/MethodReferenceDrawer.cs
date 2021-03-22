using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TypeInspector.Editor
{
    [CustomPropertyDrawer(typeof(MethodReference))]
    public class MethodReferenceDrawer : PropertyReferenceDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("TargetType");
            var propertyNameSP = property.FindPropertyRelative("MethodName");

            var propertyRect = position;
            propertyRect.height = GetPropertyHeight(property, label);
            EditorGUI.BeginProperty(propertyRect, label, property);

            position.height = base.GetPropertyHeight(typeProperty, new GUIContent(""));
            
            var objectPos = position;
            
            TypeReferenceDrawer.FilterObjectProperty = property;
            
            EditorGUI.PropertyField(position, typeProperty);
            
            TypeReferenceDrawer.FilterObjectProperty = null;

            objectPos.y += objectPos.height + 2;
            objectPos.x += 5;
            objectPos.width -= 5;

            var typeAccessor = (TypeReference) GetTargetObjectOfProperty(typeProperty);

            if (typeAccessor.IsValid())
            {
                DrawMethodSelector(
                    objectPos,
                    propertyNameSP, property, typeAccessor.Get());
            }

            EditorGUI.EndProperty();
        }

        private void DrawMethodSelector(Rect objectPos, SerializedProperty propertyNameSP, SerializedProperty root, Type targetType)
        {
            var methods = FilterMethods(targetType, root);

            if (!methods.Any())
            {
                propertyNameSP.stringValue = "";
                GUI.Label(objectPos, "Selected type not contain any public methods");
                return;
            }

            var selected = string.IsNullOrEmpty(propertyNameSP.stringValue)
                ? 0
                : methods.Select(p => p.Name).ToList().IndexOf(propertyNameSP.stringValue);

            var methodNames = methods.Select(GetMethodName).ToArray();

            var index = EditorGUI.Popup(objectPos, "Method", selected, methodNames);

            propertyNameSP.stringValue = methods.ElementAt(Mathf.Max(index, 0)).Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeAccessor = (TypeReference) GetTargetObjectOfProperty(property.FindPropertyRelative("TargetType"));

            if (typeAccessor != null && typeAccessor.IsValid())
            {
                return base.GetPropertyHeight(property, label) * 2 + 4;
            }
            
            return base.GetPropertyHeight(property, label);
        }

        public string GetMethodName(MethodInfo method)
        {
            return $"{method.Name}({ParamString(method.GetParameters())}): {method.ReturnType.FullName}";

            string ParamString(ParameterInfo[] param)
            {
                if (param.Length == 0)
                {
                    return "";
                }

                return param.Select(s => $"{s.Name}: {s.ParameterType.Name}")
                    .Aggregate((acc, seed) => acc += $", {seed}");
            }
        }
    }
}