  var originalLineDraw = Chart.controllers.horizontalBar.prototype.draw;
  Chart.helpers.extend(Chart.controllers.horizontalBar.prototype, {
  
      draw: function () {
          originalLineDraw.apply(this, arguments);
          var strokeColors = ['purple', 'orange', 'magenta', 'green', 'red', 'blue', 'grey', 'black', 'dark green', 'dark grey']
          var chart = this.chart;
          var ctx = chart.chart.ctx;
  
          var indexes = chart.config.options.lineAtIndexes;
          if (indexes)
              for (var x = 0; x < indexes.length; x++) 
              {
                var index = indexes[x].index;
                var color = indexes[x].color ? indexes[x].color : strokeColors[x];
                var label = indexes[x].label;
                if (index) {
    
                    var xaxis = chart.scales['x-axis-0'];
                    var yaxis = chart.scales['y-axis-0'];
    
                    var x1 = xaxis.getPixelForValue(index);                       
                    var y1 = chart.height - yaxis.height - 25;                                                   
    
                    var x2 = xaxis.getPixelForValue(index);                       
                    var y2 = chart.height - 25;
    
                    ctx.save();
                    ctx.beginPath();
                    ctx.moveTo(x1, y1);
                
				    if (label) {
				        ctx.textAlign = 'left';
				        ctx.font = 'bold 18px "Helvetica Nue" Helvetica'
					    ctx.fillStyle = color;
					    ctx.fillText(label + ' : ' + index , x1 + 5, y1 + ((x+1)*20));
				    }
                    ctx.strokeStyle = color;
                
                    ctx.lineTo(x2, y2);
                    ctx.stroke();
                
                    ctx.restore();
                }
              }
      }
  });