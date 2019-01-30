import { Component } from '@angular/core';
import { ApiConnectionService } from '../api-connection.service';
import { MealsVsLunchPlans, Convert } from '../meals-vs-lunch-plan';
import { ModalController } from '@ionic/angular';
import { ModalComponent } from '../modal/modal.component'
@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {
  private lunchPlans: MealsVsLunchPlans[];
  constructor(private api: ApiConnectionService, public modalController: ModalController) { 
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
   async edit(MealsVsLunchPlans: MealsVsLunchPlans) {
    const modal = await this.modalController.create({
      component: ModalComponent,
      componentProps: { api: this.api, MvL: MealsVsLunchPlans }
    });
    await modal.present();
    const { data } = await modal.onDidDismiss();
    if(data){
      this.api.Edit(data.data as MealsVsLunchPlans).subscribe(lunchPlan => this.lunchPlans = lunchPlan)
    }
  }
}
