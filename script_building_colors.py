import colorsys

def print_color(name, r, g, b):
    hex_code = f'#{int(r*255):02X}{int(g*255):02X}{int(b*255):02X}'
    print(f'{name}: {hex_code}')

print_color('Building_Top', 0.96862745, 0.8352941, 0.5764706)
print_color('Building_Base', 0.972549, 0.78431374, 0.41568628)
print_color('Building_Side', 0.627451, 0.34509805, 0.39607844)
