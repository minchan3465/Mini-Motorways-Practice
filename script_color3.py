import colorsys
import math

def srgb_to_linear(c):
    if c <= 0.04045:
        return c / 12.92
    else:
        return math.pow((c + 0.055) / 1.055, 2.4)

def linear_to_srgb(c):
    if c <= 0.0031308:
        return c * 12.92
    else:
        return 1.055 * math.pow(c, 1/2.4) - 0.055

def apply_only_sat_mult(r, g, b, sat_mult=0.821):
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    s = s * sat_mult
    r_lin, g_lin, b_lin = colorsys.hsv_to_rgb(h, s, v)
    
    r_srgb = linear_to_srgb(r_lin)
    g_srgb = linear_to_srgb(g_lin)
    b_srgb = linear_to_srgb(b_lin)
    
    return f'#{int(r_srgb*255):02X}{int(g_srgb*255):02X}{int(b_srgb*255):02X}'

colors = {
    'Water Base': (0.6039216, 0.9098039, 0.8432034),
    'Water Grid': (0.56287825, 0.8584906, 0.7599532),
    'Beach': (1, 0.9775943, 0.9372549),
    'Grass': (0.9294118, 0.909804, 0.54901963),
    'Land': (1, 0.9490197, 0.86274517),
    'Road Outline': (0.936, 0.88497657, 0.796536),
    'Building Group A': (0.98823535, 0.7490196, 0.35686275),
}

print('Colors with ONLY Saturation reduction (Keeping original brightness):')
for name, (r, g, b) in colors.items():
    print(f'{name}: {apply_only_sat_mult(r, g, b)}')
