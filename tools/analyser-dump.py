import collections
import json
import re
import sys
from pathlib import Path

path = Path(sys.argv[1])
data = json.loads(path.read_text(encoding="utf-8"))
print("FENETRES", len(data["fenetres"]))
for window in data["fenetres"]:
    print("---")
    print("classe", window.get("classe"))
    print("dpi", window.get("dpi"), "echelle", window.get("echellePourcent"))
    print("rect", window.get("largeur"), "x", window.get("hauteur"))
    print("n", window.get("nombreElements"))
    types = collections.Counter(e["TypeControle"] for e in window["elements"])
    print("types", types.most_common(12))
    print("editeurs", sum(1 for e in window["elements"] if e["EstEditeur"]))
    chrome = set()
    for element in window["elements"]:
        name = element["Nom"]
        if not name or element["EstEditeur"]:
            continue
        if element["TypeControle"] not in {"Button", "MenuItem", "TabItem", "Hyperlink", "Text"}:
            continue
        if len(name) > 42:
            continue
        if re.search(r"\d+[mh]$", name):
            continue
        chrome.add(f"{element['TypeControle']}: {name}")
    print("CHROME")
    for item in sorted(chrome):
        print(item)
