import { Injectable, } from '@angular/core';
import { HttpClient } from '@angular/common/http';import { catchError, map, tap } from 'rxjs/operators';
import { Convert, MealsVsLunchPlans } from './meals-vs-lunch-plan'
import { Observable, of, BehaviorSubject } from 'rxjs';

const httpOptions = {
  headers: new Headers({ 'Content-Type': 'application/json' })
};


@Injectable({
  providedIn: 'root'
})
export class ApiConnectionService {
  private CurrUser: any;
  private mealsVsLunchPlan : MealsVsLunchPlans[];
  private mealsVsLunchPlanApi : string = 'https://webapiinfoscreenaspit.azurewebsites.net/api/ViewMealsVsLunchPlansJoins'
  constructor(private http: HttpClient) { }

  public GetLunchPlan() : Observable<MealsVsLunchPlans[]> {
    let temp : BehaviorSubject<MealsVsLunchPlans[]> = new BehaviorSubject<MealsVsLunchPlans[]>(this.mealsVsLunchPlan);
    this.http.get<MealsVsLunchPlans[]>(this.mealsVsLunchPlanApi, {responseType: 'json'}).subscribe(lunchPlan => { this.mealsVsLunchPlan = Convert.toMealsVsLunchPlans(JSON.stringify(lunchPlan)); temp.next(lunchPlan)}, error => console.log(error), () => {  temp.next(this.mealsVsLunchPlan); console.log(this.mealsVsLunchPlan) })
    return temp.asObservable();
  }
}
