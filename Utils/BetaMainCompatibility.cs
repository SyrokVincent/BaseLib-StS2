using System.Reflection;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Modding;

namespace BaseLib.Utils;

public class BetaMainCompatibility
{
    public static class Renamed
    {
        public static VariableReference<IEnumerable<Mod>> 
            LoadedMods = new(typeof(ModManager), "LoadedMods", "GetLoadedMods()");
        
        public static VariableReference<StringName>
            FontSize = new(typeof(ThemeConstants.Label), "FontSize", "fontSize");
        public static VariableReference<StringName>
            Font = new(typeof(ThemeConstants.Label), "Font", "font");
        public static VariableReference<StringName>
            LineSpacing = new(typeof(ThemeConstants.Label), "LineSpacing", "lineSpacing");
        public static VariableReference<StringName>
            OutlineSize = new(typeof(ThemeConstants.Label), "OutlineSize", "outlineSize");
        public static VariableReference<StringName>
            FontColor = new(typeof(ThemeConstants.Label), "FontColor", "fontColor");
        public static VariableReference<StringName>
            FontOutlineColor = new(typeof(ThemeConstants.Label), "FontOutlineColor", "fontOutlineColor");
        public static VariableReference<StringName>
            FontShadowColor = new(typeof(ThemeConstants.Label), "FontShadowColor", "fontShadowColor");
    }



    /// <summary>
    /// Reference to a field/property/method with multiple possible names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableReference<T>
    {
        private Func<object?, T?> _get;
        
        public static implicit operator T(VariableReference<T> obj)
        {
            return obj._get.Invoke(null)!;
        }

        public T Get(object? obj = null)
        {
            return _get.Invoke(obj)!;
        }
        
        public VariableReference(params (Type, string)[] possibleReferences)
        {
            foreach (var entry in possibleReferences)
            {
                var func = TryName(entry.Item1, entry.Item2);
                if (func == null) continue;
                
                _get = func;
                return;
            }
            throw new Exception(
                $"Unable to find any field or property of type {typeof(T)} from set {string.Join(",", possibleReferences)}");
        }
        
        public VariableReference(Type definingType, params string[] possibleNames)
        {
            foreach (var name in possibleNames)
            {
                var func = TryName(definingType, name);
                if (func == null) continue;
                
                _get = func;
                return;
            }

            throw new Exception(
                $"Unable to find any field or property of type {typeof(T)} with name in \'{string.Join(",", possibleNames)}\' in type {definingType.FullName}");
        }

        private Func<object?, T?>? TryName(Type t, string name)
        {
            if (name.EndsWith("()")) //method
            {
                var method = t.GetMethod(name[..^2], BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (method == null) return null;
                
                if (method.GetParameters().Length > 0) throw new Exception("VariableReference only supports no-param methods; use VariableMethod instead");
                return obj => (T?) method.Invoke(obj, []);
            }
            
            var field = t.GetField(name);
            if (field != null)
            {
                return obj => (T?)field.GetValue(obj);
            }

            var prop = t.GetProperty(name);
            if (prop != null)
            {
                return obj => (T?)prop.GetValue(obj);
            }

            return null;
        }
    }
}