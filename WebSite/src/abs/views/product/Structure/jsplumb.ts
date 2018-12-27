const jsplumb = require('jsplumb').jsPlumb;
const waterfall = require('./waterfall');
const AddNode = waterfall.AddNode;
const CalcXTotalDepth = waterfall.CalcXTotalDepth;
const CalcYTotalDepth = waterfall.CalcYTotalDepth;

// CalcXTotalDepth, CalcYTotalDepth, AddNode, ConnectNode, jsPlumb
export function cashflowChart(e: any) {
  const waterfallDom = document.getElementById('waterfall') as HTMLElement;

  if (e) {
    var parent = e,
      l = `<div id="node_${parent.id}" class="generalNode" style="top:0%;left:40%">${parent.name}</div>`;

    waterfallDom.insertAdjacentHTML('beforeend', l);
    var xMaxNum = CalcXTotalDepth(parent);
    var yMaxNum = CalcYTotalDepth(parent);
    var xTotal = xMaxNum * 2 + 1;
    var xEach = Math.floor(100 / xTotal);

    var yTotal = yMaxNum * 2 + 1;
    var yEach = Math.floor(10000 / yTotal) / 100;

    AddNode(
      parent.next,
      xEach,
      yEach,
      xTotal,
      yTotal,
      parent,
      42,
      parent,
      null
    );
    
    waterfallDom.style.height = 100 * yMaxNum + 'px';

    jsplumb.draggable(document.querySelectorAll('.generalNode'));

    jsplumb.ready(function() {
      var lineStyle = {
        lineWidth: 2,
        stroke: '#818AA3'
      };

      jsplumb.importDefaults({
        DragOptions: {
          zIndex: 50000,
          cursor: 'pointer',
          clone: true
        },
        EndpointStyles: {
          width: 10,
          height: 10,
          cursor: 'pointer'
        },
        Connector: 'Straight',
        Endpoint: 'Rectangle',
        Anchor: 'Top',
        MaxConnections: 1e3,
        ConnectionsDetachable: !1,
        HoverPaintStyle: lineStyle,
        connectorOverlays: [
          [
            'Arrow',
            {
              width: 10,
              length: 10,
              location: 1
            }
          ]
        ]
      });
      const t = {
        paintStyle: {
          fill: '#B4B9C5'
        },
        connectorStyle: {
          stroke: '#B4B9C5',
          lineWidth: 2
        },
        connector: [
          'Flowchart',
          {
            stub: [0, 20],
            alwaysRespectStubs: true,
            midpoint: 0.98,
            gap: 5,
            cornerRadius: 10
          }
        ],
        isSource: !0,
        isTarget: !0,
        endpoint: 'Blank',
        connectorOverlays: [
          [
            'Arrow',
            {
              width: 10,
              length: 10,
              location: 1
            }
          ]
        ]
      };

      const n = jsplumb.addEndpoint('node_' + parent.id, {
        anchor: 'Bottom'
      }, t);

      ConnectNode(n, parent.next, t);

      jsplumb.setContainer(document.getElementById('waterfall') as HTMLElement);
    });
  }
}

// ConnectNode, jsPlumb
export function  ConnectNode(e: any, t: any, n: any) {
  if (t && t.length !== 0) {
    for (var a = 0; a < t.length; a++) {
      const l = t[a];
      const r = jsplumb.addEndpoint('node_' + l.id, n);

      jsplumb.connect({
        source: e,
        target: r
      });
      var o = jsplumb.addEndpoint(
        'node_' + l.id,
        {
          anchor: 'Bottom'
        },
        n
      );
      ConnectNode(o, l.next, n);
    }
  }
    
}