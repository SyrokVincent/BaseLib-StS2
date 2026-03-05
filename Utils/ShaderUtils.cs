using Godot;

namespace BaseLib.Utils;

public class ShaderUtils
{
    /// <summary>
    /// Convenience method to quickly generate a ShaderMaterial using the included hsv shader
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static ShaderMaterial GenerateHsv(float h, float s, float v)
    {
        var material = new ShaderMaterial()
        {
            Shader = (Shader)GD.Load<Shader>("res://shaders/hsv.gdshader").Duplicate()
        };

        material.SetShaderParameter("h", h);
        material.SetShaderParameter("s", s);
        material.SetShaderParameter("v", v);

        return material;
    }
}
