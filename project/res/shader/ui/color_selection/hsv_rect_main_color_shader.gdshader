shader_type canvas_item;

uniform float hue; // This should be between 0 and 1

vec3 HSVtoRGB(vec3 hsv) {
    float h = hsv.x * 360.0; // Convert hue to 0-360 range
    if(h == 360.0) h = 0.0;
	float s = hsv.y;
    float v = hsv.z;
    float hh = h / 60.0;
    int i = int(hh) % 6;
    float ff = hh - float(i);
    float p = v * (1.0 - s);
    float q = v * (1.0 - (s * ff));
    float t = v * (1.0 - (s * (1.0 - ff)));

    vec3 rgb;

    if (i == 0) {
        rgb = vec3(v, t, p);
    } else if (i == 1) {
        rgb = vec3(q, v, p);
    } else if (i == 2) {
        rgb = vec3(p, v, t);
    } else if (i == 3) {
        rgb = vec3(p, q, v);
    } else if (i == 4) {
        rgb = vec3(t, p, v);
    } else {
        rgb = vec3(v, p, q);
    }

    return rgb;
}

void fragment() {
    float s = UV.x; // Saturation varies with x coordinate
    float v = 1.0 - UV.y; // Value varies with y coordinate (inverted)
    
    float input_hue = mod(hue, 1.0);

    COLOR.a = 1.0;
    COLOR.rgb = HSVtoRGB(vec3(input_hue, s, v));
}