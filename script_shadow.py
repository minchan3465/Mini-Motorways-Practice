def convert_float_to_hex(r, g, b, a=1.0):
    return f'#{int(r*255):02X}{int(g*255):02X}{int(b*255):02X}'

print("Shadow Colors:")
print(f"Shadow Environment Theme: {convert_float_to_hex(0.21506765, 0.23290256, 0.254717)} with Alpha {int(0.16078432*255)}")
print(f"Shadow Material: {convert_float_to_hex(0.25845498, 0.36104187, 0.4528302)} with Alpha {int(0.32156864*255)}")
print(f"Drop Shadow (Water): {convert_float_to_hex(0.3721075, 0.6981132, 0.6616557)} with Alpha {int(0.6156863*255)}")
