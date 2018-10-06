function buckweatt(m, q) {
  var p = m_prefix(m, q);
  if (map_list[m].encoding == 0) return q;
  if (map_list[m].encoding != 2) return p + q;
  if (p.charAt(0) != "F")
    return q;
  var w = q,
      f = v = "";
  if (q.length > 16) w = q.substr(q.length - 16, 16);
  var r = String(Number(w) + 10 * m * (Number(w.charAt(w.length - 1)) + 1));
  if (q.length > 16) {
    f = q.substr(0, q.length - 16);
    v = "000000".substr(0, 16 - r.length);
  }
  return p.replace("FF", "FE") + f + v + r
}

function set_prefix(str) {
  return "FF" + huffman(" " + str).substr(0, 4) + "_"
}

function m_prefix(mcode, str) {
  if (map_list[mcode].encoding) {
    code = (mcode % 2) ? "1" : "0";
    if ((str.substr(str.length - 1, 1) == code) || (str.substr(str.length - 3, 1) == "0")) return set_prefix(str + ".jpg")
  }
  return ""
}
