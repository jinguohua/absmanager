Object.defineProperty(exports, "__esModule", { value: true });
// CalcTotalBranch, CalcXAxis, CalcYDepth, AddNode
function AddNode(
  node,
  xEach,
  yEach,
  xTotal,
  yTotal,
  root,
  xParent,
  parent,
  rootBranch
) {
  for (var i = 0; i < node.length; i++) {
    var siblingBranchNum = 0;
    var selfBranchNum = CalcTotalBranch(node[i]);
    if (node.length >= 2) {
      for (var j = 0; j < node.length; j++) {
        if (node[j].id != node[i].id) {
          siblingBranchNum = CalcTotalBranch(node[j]);
        }
      }
      if (i == 2) {
        siblingBranchNum =
          CalcTotalBranch(node[0]) + CalcTotalBranch(node[1]) + 1;
        rootBranch = node[i];
      }
    }
    var ylevel = CalcYDepth(node[i], root);
    var sum = 0;
    var cnt = node[i].next.length;
    var son = node[i].next;
    if (son != null && son.length > 0) {
      for (var m = 0; m < cnt; m++) {
        sum += CalcXAxis(son[m], root, parent);
      }
    }
    var x1 = cnt > 0 ? sum / cnt : 0;
    var x2 = CalcXAxis(node[i], root, parent);
    var x_parent = (xParent / xEach + 1) / 2;
    var x = x1 > x2 ? x1 : x2;
    if (rootBranch != null && rootBranch.id == 102) {
      x = x > x_parent ? x : x_parent;
    }
    if (
      rootBranch != null &&
      rootBranch.id != 102 &&
      node[i].parent_id == rootBranch.id
    ) {
      x = x > x_parent ? x : x_parent;
    }
    if (
      i == 0 &&
      selfBranchNum >= 1 &&
      siblingBranchNum >= 1 &&
      ylevel < 6
    ) {
      x = 1;
    } else if (siblingBranchNum >= 1 && ylevel < 6) {
      x = 3;
    }
    if (siblingBranchNum >= 2) {
      x = 4;
    }
    if (node[i].id == 102) {
      x = 4;
      if (siblingBranchNum >= 3) {
        x = 5;
      }
      if (siblingBranchNum == 1) {
        x = 3;
      }
      if (siblingBranchNum == 0) {
        x = 2;
      }
    }
    x = (x * 2 - 1) * xEach;

    var y = yEach * (2 * ylevel);

    var tooltip = "无";
    if (node[i].name.indexOf("事件判断") >= 0) {
      tooltip = "条件:\t" + node[i].condition;
    }

    if (node[i].name.indexOf("事件判断") < 0) {
      if (node[i].legal.length > 0) {
        tooltip = node[i].legal;
      } else {
        tooltip = node[i].name;
      }
    }
    if (node[i].name.length > 20) {
      var origName = node[i].name;
      node[i].name = node[i].name.substr(0, 19) + "...";

      if (node[i].legal.length > 0) {
        tooltip = node[i].legal;
      } else {
        tooltip = origName;
      }
    }
    var html =
      "<div id='node_" +
      node[i].id +
      "' class=\"generalNode\" style=' position: absolute; top:" +
      parseInt(y) +
      "%;left:" +
      parseInt(x) +
      "%' title=\"" +
      tooltip +
      '">' +
      node[i].name +
      "</div>";
    document.getElementById("waterfall").insertAdjacentHTML('beforeend', html);

    // $("#waterfall").append(html);

    var child = node[i].next;
    if (node[i].id > 100) {
      rootBranch = node[i];
    }
    AddNode(
      child,
      xEach,
      yEach,
      xTotal,
      yTotal,
      root,
      x,
      node[i],
      rootBranch
    );
  }
}

function CalcXAxis(node, root, base) {
  var x = 0;
  var y1 = CalcYDepth(node, base);
  var y2 = CalcYDepth(root, base);
  var y = y1 - y2 + 1;
  if (y == 2) {
    var son = root.next;
    if (son != null && son.length > 0) {
      for (var k = 0; k < son.length; k++) {
        x--;

        if (node.id == son[k].id) {
          x *= -1;
          break;
        }
      }
    } else {
      x = x--;
    }
  }
  if (2 < y) {
    var son = root.next;
    if (son != null && son.length > 0) {
      for (var k = 0; k < son.length; k++) {
        var temp = CalcXAxis(node, son[k], base);
        if (temp > 0) {
          x *= -1;
          x += temp;
          return x;
        } else if (temp < 0) {
          x = temp - k;
        }
      }
    } else {
      x--;
    }
  }
  if (node.id == root.id) {
    return 1;
  } else {
    return x;
  }
}

function CalcXDepth(e, t) {
  if (1 == e) return 1;
  var n = 1,
    a = 0,
    l = t.next;
  if (null != l && l.length > 0 && (n++, n == e && (a = l.length), e > n))
    for (var r = 0; r < l.length; r++) a += CalcXDepth(e - 1, l[r]);
  return a;
}

function CalcXTotalDepth(e) {
  var t = 0,
    n = 0,
    a = e.next;
  if (null != a && a.length > 0)
    for (var l = 0; l < a.length; l++) (n = CalcXTotalDepth(a[l])), (t += n);
  return 0 == t && (t = 1), t;
}

function CalcYTotalDepth(e) {
  var t = 0,
    n = e.next;
  if (null != n && n.length > 0)
    for (var a = 0; a < n.length; a++) {
      var l = CalcYTotalDepth(n[a]);
      a > 0 ? (t = t > l ? t : l) : (t += l);
    }
  return t++, t;
}

function CalcYDepth(e, t) {
  var n = -1;
  if (e.id == t.id) return 1;
  var a = t.next;
  if (null != a && a.length > 0)
    for (var l = 0; l < a.length; l++) {
      var r = CalcYDepth(e, a[l]);
      r > 0 && ((n *= -1), (n += r));
    }
  return n;
}

function CalcTotalBranch(root) {
  var rt = 0;
  if (root.next.length == 2) {
    rt += 1;
  }
  if (root.next.length == 3) {
    rt += 2;
  }
  for (var i = 0; i < root.next.length; i++) {
    rt += CalcTotalBranch(root.next[i]);
  }
  return rt;
}


exports.AddNode = AddNode;
exports.CalcXTotalDepth = CalcXTotalDepth;
exports.CalcYTotalDepth = CalcYTotalDepth;
