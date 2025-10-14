shader_type canvas_item;

uniform float outline_width : hint_range(0.0, 10.0) = 1.0;
uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);

void fragment() {
    vec4 color = texture(TEXTURE, UV);
    vec2 size = TEXTURE_PIXEL_SIZE * outline_width;
    
    // Sample surrounding pixels for outline
    float outline = 0.0;
    outline += texture(TEXTURE, UV + vec2(-size.x, 0)).a;
    outline += texture(TEXTURE, UV + vec2(size.x, 0)).a;
    outline += texture(TEXTURE, UV + vec2(0, -size.y)).a;
    outline += texture(TEXTURE, UV + vec2(0, size.y)).a;
    outline += texture(TEXTURE, UV + vec2(-size.x, -size.y)).a;
    outline += texture(TEXTURE, UV + vec2(size.x, -size.y)).a;
    outline += texture(TEXTURE, UV + vec2(-size.x, size.y)).a;
    outline += texture(TEXTURE, UV + vec2(size.x, size.y)).a;
    
    outline = min(outline, 1.0);
    
    // Mix original color with outline
    COLOR = mix(color, outline_color, outline * (1.0 - color.a));
}