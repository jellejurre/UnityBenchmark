import sys

import matplotlib.pyplot as plt
import numpy as np
from scipy.interpolate import griddata
def polyfit2d(x, y, z, kx=3, ky=3, order=None):
    x, y = np.meshgrid(x, y)
    # coefficient array, up to x^kx, y^ky
    coeffs = np.ones((kx+1, ky+1))

    # solve array
    a = np.zeros((coeffs.size, x.size))

    # for each coefficient produce array x^i, y^j
    for index, (j, i) in enumerate(np.ndindex(coeffs.shape)):
        # do not include powers greater than order
        if order is not None and i + j > order:
            arr = np.zeros_like(x)
        else:
            arr = coeffs[i, j] * x**i * y**j
        a[index] = arr.ravel()

    # do leastsq fitting and return leastsq result
    return np.linalg.lstsq(a.T, np.ravel(z), rcond=None)

f = open("Assets/jellejurre/Benchmarker/Output/" + sys.argv[2] + "/"+ sys.argv[1] + ".txt", "r")
lines = f.readlines()
names = lines[0].split(",") 
percentage = float(sys.argv[4])
lines.pop(0)
if len(names) == 1:
    xs = [int(spl.split(",")[0]) for spl in lines]
    ys = [float(spl.split(",")[1]) for spl in lines]
    maxVal = max(xs)
    if percentage != 0:
        newLines = []
        for i in range(len(xs)):
            if(xs[i] < maxVal * percentage):
                newLines.append(lines[i])
        lines = newLines
    xs = [int(spl.split(",")[0]) for spl in lines]
    ys = [float(spl.split(",")[1]) for spl in lines]
    plt.plot(xs, ys)
    vars = np.polyfit(xs, ys, 2)
    x = np.linspace(min(xs),max(xs),1000)
    y = [np.polyval(vars, i) for i in x]
    if(sys.argv[3] == "True"):
        plt.plot(x, y)
    plt.xlabel(names[0])
    plt.ylabel("Time in seconds")
    vars = ["{:.3E}".format(var) for var in vars]
    plt.title(f"Fitted Curve: {vars[0]}x^2 + {vars[1]}x + {vars[2]}")
    plt.savefig("Assets/jellejurre/Benchmarker/Output/" + sys.argv[2] + "/Graphs/" + sys.argv[1] + ".png",  bbox_inches="tight")
    f = open("Assets/jellejurre/Benchmarker/Output/" + sys.argv[2] + "/Data/" + sys.argv[1] + ".txt", "w")
    f.write(f"{vars[0]},{vars[1]},{vars[2]}")
    f.close()
else: 
    x1s = np.array([int(spl.split(",")[0]) for spl in lines])
    x2s = np.array([int(spl.split(",")[1]) for spl in lines])
    ys = np.array([float(spl.split(",")[2]) for spl in lines])
    fig, ax = plt.subplots()
    im = ax.scatter(x1s, x2s, c=ys)
    fig.colorbar(im, ax=ax)
    A = np.array([np.ones(x1s.shape), x2s, x2s**2, x1s, x1s*x2s, x1s*x2s**2, x1s**2, x1s**2*x2s, (x1s**2)*(x2s**2)]).T
    B = ys.flatten()
    coeff, r, rank, s = np.linalg.lstsq(A, B, rcond = None)
    plt.xlabel(names[0])
    plt.ylabel(names[1])
    vars = ["{:.3E}".format(var) for var in coeff]
    f = open("Assets/jellejurre/Benchmarker/Output/" + sys.argv[2] + "/Data/" + sys.argv[1] + ".txt", "w")
    f.write(",".join(vars))
    f.close()
    x1dis = np.unique(x1s)
    x2dis = np.unique(x2s)
    z = np.zeros((x1dis.size, x2dis.size))
    for i in range(len(ys)):
        z[x1dis.tolist().index(x1s[i]),x2dis.tolist().index(x2s[i])] = ys[i]
   
    if (sys.argv[3]=="True"):
        plt.contour(x1dis, x2dis, z.T)
    plt.rcParams["axes.titlesize"] = 10
    plt.title(f"Fitted Curve: {vars[0]} + {vars[1]}y + {vars[2]}y^2\n + {vars[3]}x + {vars[4]}x*y + {vars[5]}x*y^2\n + {vars[6]}x^2 + {vars[7]}x^2*y + {vars[8]}x^2 * y^2", wrap=True)
    plt.savefig("Assets/jellejurre/Benchmarker/Output/" + sys.argv[2] + "/Graphs/" + sys.argv[1] + ".png", bbox_inches="tight")