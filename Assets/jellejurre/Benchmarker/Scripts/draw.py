import sys

import matplotlib.pyplot as plt
import numpy as np

f = open("Assets/jellejurre/Benchmarker/Output/" + sys.argv[1] + ".txt", "r")
lines = f.readlines()
xs = [int(spl.split(",")[0]) for spl in lines]
ys = [float(spl.split(",")[1]) for spl in lines]

plt.plot(xs, ys)

vars = np.polyfit(xs, ys, 2)
x = np.linspace(min(xs),max(xs),1000)
y = [np.polyval(vars, i) for i in x]
plt.plot(x, y)
plt.xlabel("Iteration Number")
plt.ylabel("Time in ms")
vars = ["{:.3E}".format(var) for var in vars]
plt.title(f"Fitted Curve: {vars[0]}x^2 + {vars[1]}x + {vars[2]}")
plt.savefig("Assets/jellejurre/Benchmarker/Output/Graphs/" + sys.argv[1] + ".png")
print("saved")