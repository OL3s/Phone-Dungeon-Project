shader_type canvas_item;

// --- local criss-cross waves (independent layers) ---
uniform float local_amp    : hint_range(0.0, 0.6) = 1.0;
uniform float local_freq   : hint_range(0.2, 12.0) = 1.0;
uniform float local_speed  : hint_range(0.0, 6.0)  = 4;
uniform int   local_layers = 5;

// --- popping/roaming blobs (circular warps) ---
uniform float blob_rate     : hint_range(0.0, 8.0) = 0.0;   // spawns/sec per stream
uniform float blob_strength : hint_range(0.0, 0.3) = 0.0;  // displacement power
uniform float blob_scale    : hint_range(0.02,1.0) = 0.0;  // base radius (screen)
uniform float blob_life     : hint_range(0.2, 5.0) = 0.0;   // seconds
uniform int   blob_streams  = 10;                            // 1..8

// --- gentle global swell on top (big wave) ---
uniform float global_amp    : hint_range(0.0, 0.4) = 0;
uniform float global_freq   : hint_range(0.1, 6.0) = 0;
uniform float global_speed  : hint_range(0.0, 4.0) = 0;

// hash utils
float hash11(float n){ return fract(sin(n)*43758.5453123); }
float hash21(vec2 p){ return fract(sin(dot(p, vec2(41.32,289.32))) * 43758.5453); }
vec2  hash2(vec2 p){ float h=hash21(p); float a=6.2831853*h; return vec2(cos(a), sin(a)); }

// soft circle mask
float soft_circle(vec2 d, float r, float w){
    float t = length(d) - r;
    return 1.0 - smoothstep(-w, w, t);
}

void fragment(){
    vec2 uv  = UV;
    vec2 suv = SCREEN_UV;

    // ----- LOCAL WAVES (independent layers) -----
    vec2 disp_local = vec2(0.0);
    vec2 field = suv * local_freq;
    for (int i = 0; i < 8; i++){
        if (i >= local_layers) break;
        float fi = float(i);
        float a  = hash21(vec2(fi, 0.37)) * 6.2831853;
        vec2 dir = vec2(cos(a), sin(a));
        float sp = 0.5 + hash21(vec2(fi, 2.7));
        float w  = sin(dot(field, dir) + TIME * local_speed * sp);
        float gate = 0.5 + 0.5 * sin(dot(field, vec2(-dir.y, dir.x)) * 1.8 + TIME * (0.4*local_speed + 0.2*sp));
        disp_local += dir * w * gate;
    }
    disp_local *= local_amp / float(max(local_layers,1));

    // ----- BLOBS (procedural popping lenses) -----
    vec2 disp_blob = vec2(0.0);
    float period = 1.0 / max(blob_rate, 0.001);
    for (int s = 0; s < 8; s++){
        if (s >= blob_streams) break;
        float fs = float(s);
        float tphase = TIME / period + fs*7.19;
        float k = floor(tphase);
        float ft = fract(tphase);                 // 0..1
        // ease-in, hold, ease-out
        float life = smoothstep(0.05, 0.35, ft) * (1.0 - smoothstep(0.6, 1.0, ft));

        // random screen-space center + slight drift per bubble
        vec2 center = vec2(hash11(k*12.7+fs*1.3), hash11(k*17.9+fs*2.1));
        center += (hash2(vec2(k*3.1+2.0, fs*5.3)) - vec2(0.0)) * 0.02;

        float radius = blob_scale * (0.7 + 0.9*hash11(k*5.7+fs*3.9)) * (0.6 + 0.7*ft);
        vec2 d = suv - center;

        float core = soft_circle(d, radius, 0.015);
        float rim  = soft_circle(d, radius, 0.006) - core;
        float mask = clamp(core + rim*1.4, 0.0, 1.0) * life;

        vec2 tangent = vec2(-d.y, d.x);
        float swirl  = sign(hash11(k+3.14)-0.5) * 0.4;
        vec2 lens = normalize(d) * 0.9 + normalize(tangent) * swirl;
        disp_blob += lens * (blob_strength * mask);
    }

    // ----- GLOBAL SWELL (large, gentle) -----
    float gx = sin(suv.y * global_freq + TIME * global_speed);
    float gy = cos(suv.x * (global_freq*0.7) - TIME * global_speed * 0.9);
    vec2 disp_global = vec2(gx, gy) * global_amp;

    // ----- COMBINE -----
    vec2 disp = disp_local + disp_blob + disp_global;

    COLOR = texture(TEXTURE, uv + disp);
}
