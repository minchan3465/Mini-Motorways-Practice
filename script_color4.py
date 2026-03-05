import colorsys
import math

def linear_to_srgb(c):
    if c <= 0.0031308:
        return c * 12.92
    else:
        return 1.055 * math.pow(c, 1/2.4) - 0.055

def get_bright_srgb_color(r, g, b, sat_mult=0.9):
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    s = s * sat_mult # reduce saturation slightly to avoid neon look, keep V at 1.0
    
    r_lin, g_lin, b_lin = colorsys.hsv_to_rgb(h, s, v)
    
    r_srgb = linear_to_srgb(r_lin)
    g_srgb = linear_to_srgb(g_lin)
    b_srgb = linear_to_srgb(b_lin)
    
    return f'#{int(r_srgb*255):02X}{int(g_srgb*255):02X}{int(b_srgb*255):02X}'

colors = {
    'Water Base': (0.6039216, 0.9098039, 0.8432034),
    'Land': (1, 0.9490197, 0.86274517),
    'Grass': (0.9294118, 0.909804, 0.54901963),
    'Road Outline': (0.936, 0.88497657, 0.796536),
    'Building Group A': (0.98823535, 0.7490196, 0.35686275),
    'Building Group B': (0.43569016, 0.82239395, 0.94509804)
}

print('Colors with Brightness Kept (Original Float -> sRGB Conversion):')
for name, (r, g, b) in colors.items():
    print(f'{name}: {get_bright_srgb_color(r, g, b)}')
