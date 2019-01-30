import { Component } from '@angular/core';
import { ApiConnectionService } from '../api-connection.service';
import { MealsVsLunchPlans, Convert } from '../meals-vs-lunch-plan';
@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {
  private lunchPlans: MealsVsLunchPlans[];
  constructor(private api: ApiConnectionService) { 
    api.GetLunchPlan().subscribe(lunchPlans => this.lunchPlans = lunchPlans, error => console.log(error), () => { this.getLunchplansPrWeek()});
  
  }
   public getLunchplansPrWeek() : MealsVsLunchPlans[]{
     if(this.lunchPlans === undefined){
       console.log(this.lunchPlans)
       return undefined;
     }
     var lunchPlanner: any = {};
     const temp : MealsVsLunchPlans[] = [];
      this.lunchPlans.filter(function(value, index){
        if(temp.every(function(value2,index2) {
              if(value.week != value2.week){
              return true;
            }
              else{
                return false;
              }
            } 
          )
        )
        temp.push(value)
        lunchPlanner = (JSON.parse(Convert.mealsVsLunchPlansToJson(temp)));
        }
      )
      return temp;
   }
   public edit(MealsVsLunchPlans: MealsVsLunchPlans): void{
    const LunchPlans: MealsVsLunchPlans[]  = []
    LunchPlans.push(MealsVsLunchPlans)
    this.api.Edit(LunchPlans).subscribe(lunchPlans => this.lunchPlans = lunchPlans, error => console.log(error), () => { console.log(this.lunchPlans)})
    console.log(this.lunchPlans);
  }
}
