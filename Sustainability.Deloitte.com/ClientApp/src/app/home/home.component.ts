import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { cloudSubscription, rgGroup } from 'src/app/models/data.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  resourceGroups = new FormControl();
  resources = new FormControl();

  rgList: string[] = [];
  rstypeList: string[] =[];

  public emissions!: rgGroup[];
  public emissionsfiltered!: rgGroup[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<rgGroup[]>(baseUrl + 'CarbonEmission').subscribe({
      next: (result) => {
        this.emissions = result;
        console.log(this.emissions);
        this.emissionsfiltered = JSON.parse(JSON.stringify(result));
        this.emissions.forEach(rg => {
          this.rgList.push(rg.resourceGroup);
          rg.resources.forEach(rs => {
            if(!this.rstypeList.includes(rs.type)){
              this.rstypeList.push(rs.type)
            }
          })
        })
        console.log(result);
      },
      error: (error) => console.error(error),
    });
  }

  ngOnInit(): void {
    console.log(this.emissions);
  }

  optionRgSelected(event: any) {
    this.filterEmission(event.value,[]);
    console.log(event);
  }

  optionRgTypeSelected(event: any) {
    // this.filterEmission(this.rgList,this.rstypeList);

    console.log(event);
  }

  filterRsType(){
    
  }

  filterEmission(rgnameList: string[], rgTypeNameList: string[]){
    console.log(rgnameList, rgTypeNameList);
    if(rgnameList?.length ==0 ){
     this.emissionsfiltered = JSON.parse(JSON.stringify(this.emissions)) ;
    }
    else{
      this.emissionsfiltered =[];
      rgnameList.forEach(rg => {

     let temp = JSON.parse(JSON.stringify(this.emissions)) as rgGroup[];

        temp.forEach(rgg =>{
          if(rgg.resourceGroup == rg){
            this.emissionsfiltered.push(rgg);
          }
        })
      }
        
        )
    }
    console.log(this.emissionsfiltered);
  }

}
