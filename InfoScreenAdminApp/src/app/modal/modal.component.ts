import { Component, OnInit, Input } from '@angular/core';
import { NavParams, ModalController} from '@ionic/angular';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MealsVsLunchPlans } from '../meals-vs-lunch-plan';
import { ApiConnectionService } from '../api-connection.service';
@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit {
  EditedForm: FormGroup;
  loading = false;
  submitted = false;
  passedId = 0;
  @Input() MvL: MealsVsLunchPlans;
  @Input() api: ApiConnectionService
  constructor(navParams: NavParams,private formBuilder: FormBuilder,public modalController: ModalController) {
    this.api = navParams.get('api');
    this.MvL = navParams.get('MvL')
  }
  set f(value: any) {console.log(value); console.log(this.EditedForm); }
  get f() { return this.EditedForm.controls; }

  ngOnInit() {
    this.EditedForm = this.formBuilder.group({
      id: [''],
      week: ['', Validators.required],
      monday: ['', Validators.required],
      tuesday: ['', Validators.required],
      wednesday: ['', Validators.required],
      thursday: ['', Validators.required],
      friday: [''],
    });
    if(this.MvL){
      this.EditedForm.setValue({id: this.MvL.id, week: this.MvL.week ,monday: this.MvL.monday, tuesday: this.MvL.tuesday, wednesday: this.MvL.wednesday, thursday: this.MvL.thursday, friday: this.MvL.friday})
    }
  }
  onSubmit() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.EditedForm.invalid) {
        return;
    }
    this.loading = true;
    let note = this.MvL;
    note = this.EditedForm.value as MealsVsLunchPlans;
    this.modalController.dismiss({'data': note});
  }
}
