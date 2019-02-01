import { Injectable, } from '@angular/core';
import { RequestOptions,Headers,Http,Response } from '@angular/http';
import { map } from 'rxjs/operators';
import { Convert, MealsVsLunchPlans } from './meals-vs-lunch-plan'
import { Observable, BehaviorSubject } from 'rxjs';

const headers = new Headers({ 'Content-Type': 'application/json', 'Access-Control-Allow-Origin': '*'})


@Injectable({
  providedIn: 'root'
})
export class ApiConnectionService {
  private CurrUser: any;
  private mealsVsLunchPlan : MealsVsLunchPlans[];
  private mealsVsLunchPlanApi : string = 'http://localhost:3019/api/ViewMealsVsLunchPlansJoins'
  constructor(private http: Http) { }

  public GetLunchPlan() : Observable<MealsVsLunchPlans[]> {
    let temp : BehaviorSubject<MealsVsLunchPlans[]> = new BehaviorSubject<MealsVsLunchPlans[]>(this.mealsVsLunchPlan);
    this.http.get(this.mealsVsLunchPlanApi).pipe(map((res: Response) => res.json())).subscribe(lunchPlan => { this.mealsVsLunchPlan = Convert.toMealsVsLunchPlans(JSON.stringify(lunchPlan)); temp.next(lunchPlan)}, error => console.log(error), () => {  temp.next(this.mealsVsLunchPlan); console.log(this.mealsVsLunchPlan) })
    return temp.asObservable();
  }
  public Edit(MealsVsLunchPlans: MealsVsLunchPlans): Observable<MealsVsLunchPlans[]> {
    const options = new RequestOptions({headers: headers});
    console.log(MealsVsLunchPlans)
    this.http.put(this.mealsVsLunchPlanApi + '/' + MealsVsLunchPlans.week, JSON.stringify(MealsVsLunchPlans), options)
    return this.GetLunchPlan();
  }
}
