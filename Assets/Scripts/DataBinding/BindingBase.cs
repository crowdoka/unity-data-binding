namespace Crowdoka.DataBinding
{
    using UnityEngine;

#if UNITY_EDITOR
    using System;
    using System.IO;
    using NaughtyAttributes;
    using UnityEditor;
#endif

    /// <summary>
    ///     Binding base class
    /// </summary>
    public abstract class BindingBase : MonoBehaviour
    {
#if UNITY_EDITOR
        private const string AddRuntimeBindingTag = "AddRuntimeBinding";

        [Button]
        public void GenerateRuntimeBinding()
		{
			string generatedClass = GenerateClass();

			var filePath = GetRuntimeClassPath();

			AddTag();
			gameObject.tag = AddRuntimeBindingTag;

			CheckOut(filePath);
			File.WriteAllText(filePath, generatedClass);
			AssetDatabase.Refresh();
		}

        protected abstract string GenerateClass();

        protected abstract string GetRuntimeClassName();

        protected string GetRuntimeClassPath()
		{
            var className = GetRuntimeClassName();
            return Path.Combine(Application.dataPath, "Scripts", "Generated", $"{className}.cs");
        }

        protected string GetRuntimeAssetPath()
        {
            var className = GetRuntimeClassName();
            return $"Assets/Scripts/Generated/{className}.cs";
        }

        protected static void CheckOut(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            // Make path relative to project folder.
            var projectPath = Application.dataPath;
            if (path.StartsWith(projectPath) && path.Length > projectPath.Length &&
                (path[projectPath.Length] == '/' || path[projectPath.Length] == '\\'))
                path = path.Substring(0, projectPath.Length + 1);

            AssetDatabase.MakeEditable(path);
        }

        public static void AddTag()
        {
            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject so = new SerializedObject(asset[0]);
                SerializedProperty tags = so.FindProperty("tags");

                for (int i = 0; i < tags.arraySize; ++i)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == AddRuntimeBindingTag)
                    {
                        return;
                    }
                }

                tags.InsertArrayElementAtIndex(0);
                tags.GetArrayElementAtIndex(0).stringValue = AddRuntimeBindingTag;
                so.ApplyModifiedProperties();
                so.Update();
            }
        }

        protected static void OnScriptsReloaded<TBinding, TRuntimeBinding>() where TBinding : BindingBase where TRuntimeBinding : MonoBehaviour
        {
            AddTag();
            var tagged = GameObject.FindGameObjectsWithTag(AddRuntimeBindingTag);
            foreach (var toProcess in tagged)
            {
                var binding = toProcess.GetComponent<TBinding>();
                if (binding == null)
                {
                    return;
                }

                var type = Type.GetType($"Crowdoka.DataBinding.Generated.{binding.GetRuntimeClassName()}");

                if (type == null)
                {
                    return;
                }

                if (toProcess.TryGetComponent(type, out var _) == false)
                {
                    if (toProcess.TryGetComponent<TRuntimeBinding>(out var other))
                    {
                        DestroyImmediate(other);
                    }
                    toProcess.AddComponent(type);
                }

                toProcess.tag = "Untagged";
            }
        }
#endif
    }
}