Shader "AlphaTwoSided" {
        
Properties
{
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader
{
        Tags {"Queue" = "Geometry+100"}

        Pass
        {
                Alphatest Greater [_Cutoff]
                Cull off
                
                Lighting On
                Material
                {
                        Diffuse [_Color]
                        Ambient [_Color]
                }
                
                SetTexture[_MainTex] {Combine texture * primary DOUBLE, texture} 
        }
}

}