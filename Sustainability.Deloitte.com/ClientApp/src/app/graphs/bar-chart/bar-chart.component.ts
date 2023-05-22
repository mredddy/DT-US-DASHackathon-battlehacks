import { Component, Input, OnInit } from '@angular/core';
import { cloudSubscription, rgGroup } from 'src/app/models/data.model';
import * as d3 from 'd3';
//import { axisBottom, axisLeft, max, scaleBand, scaleLinear, stack } from 'd3';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.css'],
})
export class BarChartComponent implements OnInit {
  @Input() carbonEmissions!: rgGroup[];

  private svg: any;
  private margin = 50;
  private width = 600 - this.margin * 2;
  private height = 400 - this.margin * 2;

  data: rgGroup[] = [];
  public cumulativeEmissions: number = 0;
  public airplaneEmissionComparison = 2;

  private drawBars(data: any[]): void {
    // this.data = this.carbonEmissions;
    this.svg = d3
      .select('div#bar')
      .append('svg')
      .attr('width', this.width + this.margin * 2)
      .attr('height', this.height + this.margin * 2)
      .append('g')
      .attr('transform', 'translate(' + this.margin + ',' + this.margin + ')');

    let processedData: any = [];
    data.forEach((rgroup) =>
      rgroup.resources?.forEach((resource: any) => {
        resource['rgname'] = rgroup['rgname'];
        rgroup.resources = resource;
        processedData.push(resource);
      })
    );
    console.log('processedData', processedData);
    processedData.forEach((obj: any) => {
      this.cumulativeEmissions = this.cumulativeEmissions + obj.carbonEmission;
    });

    const yMax: any = d3.max(processedData, function (d: any) {
      return d.carbonEmission;
    });
    console.log(yMax);
    // Create the X-axis band scale
    const x = d3
      .scaleBand()
      .range([0, this.width])
      .domain(processedData.map((d: any) => d.name))
      .padding(0.2);

    // Draw the X-axis on the DOM
    this.svg
      .append('g')
      .attr('transform', 'translate(0,' + this.height + ')')
      .call(d3.axisBottom(x))
      .selectAll('text')
      .attr('transform', 'translate(-10,0)rotate(-45)')
      .style('text-anchor', 'end');

    // Create the Y-axis band scale
    const y = d3
      .scaleLinear()
      .domain([0, yMax * 1.25])
      .range([this.height, 0]);

    // Draw the Y-axis on the DOM
    this.svg.append('g').call(d3.axisLeft(y));

    // Create and fill the bars
    this.svg
      .selectAll('bars')
      .data(processedData)
      .enter()
      .append('rect')
      .attr('x', (d: any) => x(d.name))
      .attr('y', (d: any) => y(d.carbonEmission))
      .attr('width', x.bandwidth())
      .attr('height', (d: any) => this.height - y(d.carbonEmission))
      .attr('fill', (d: any) => {
        if (d.carbonEmission < yMax / 2) return '#228B22';
        else return '#d04a35';
      });

    //const textLabel = x.bandwidth() || 0;
    this.svg
      .selectAll('bars')
      .data(processedData)
      .enter()
      .append('text')
      .text(function (d: any) {
        return d.carbonEmission;
      })
      .attr('x', (d: any) => {
        const yy = x(d.name) || 0;
        const xx = x.bandwidth() / 2;
        return yy + xx;
      })
      .attr('y', (d: any) => y(d.carbonEmission) + 20)
      .attr('font-family', 'sans-serif')
      .attr('font-size', '11px')
      .attr('fill', 'white')
      .attr('text-anchor', 'middle')
      .attr('class', 'grid');
  }
  ngOnChanges(){
    this.cumulativeEmissions =0;
    let bar = document.getElementById('bar');
    bar?.replaceChildren();
    this.drawBars(this.carbonEmissions);
  }

  ngOnInit(): void {
    // this.createSvg();
    // this.drawBars(this.data);
    // this.drawBars(this.carbonEmissions);

    // if (this.carbonEmissions) {
    //   setTimeout(() => {
    //     console.log(this.carbonEmissions);
    //     this.drawBars(this.carbonEmissions);
    //     // window.addEventListener('resize', this.drawGraph);
    //   }, 0);
    // }
  }
}
