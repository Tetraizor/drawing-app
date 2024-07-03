shader_type canvas_item;

float fmod(float x, float y) {
	return (x / y) - trunc(x / y);
}

vec3 HSVtoRGB(vec3 hsv) {
    float h = hsv.x * 360.0, s = hsv.y, v = hsv.z;
    float hh = h / 60.0;
    int i = int(hh);
    float ff = hh - float(i);
    float p = v * (1.0 - s);
    float q = v * (1.0 - (s * ff));
    float t = v * (1.0 - (s * (1.0 - ff)));

    vec3 rgb;

    switch (i) {
        case 0:
            rgb = vec3(v, t, p);
            break;
        case 1:
            rgb = vec3(q, v, p);
            break;
        case 2:
            rgb = vec3(p, v, t);
            break;
        case 3:
            rgb = vec3(p, q, v);
            break;
        case 4:
            rgb = vec3(t, p, v);
            break;
        case 5:
        default:
            rgb = vec3(v, p, q);
			break;
    }

	return rgb;
}

void fragment() {
	COLOR.a = 1.0;
	COLOR.rgb = HSVtoRGB(vec3(UV.y, 1, 1));
}