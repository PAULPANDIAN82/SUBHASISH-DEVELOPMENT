import { IRemunerator, IBeneficiary } from './../Expences';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, NgForm, NgModel, FormBuilder, Validators } from '@angular/forms'
import { IExpense } from '../Expences';
import { ExpenseService } from '../expense.service';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';

@Component({
  selector: 'apt-addexpense',
  templateUrl: './addexpense.component.html',
  styleUrls: ['./addexpense.component.css']
})
export class AddexpenseComponent implements OnInit {

  addExpenseForm: FormGroup;

  expenseTypes: IExpenseType[] = 
  [
    {value: '0', viewValue: 'Water'},
    {value: '1', viewValue: 'Electricity'},
    {value: '2', viewValue: 'Plumber'}
  ];
  remunerators: IExpense[];
  benificiaries: IBeneficiary[];
  filteredRemunerators: Observable<IExpense[]>;
  filteredBenificiary: Observable<IBeneficiary[]>;

  constructor(private expenseService: ExpenseService, private fb: FormBuilder) {

    this.expenseService.getExpenses().subscribe(result => this.remunerators = result as IExpense[]);
    this.expenseService.getBenificiries().subscribe(result => this.benificiaries = result as IBeneficiary[]);
  }

  ngOnInit(): void {

    this.addExpenseForm = this.fb.group({
      expenseDescriptionCtrl: [''],
      expenseAmountCtrl: ['1000', Validators.required],
      beneficiaryCtrl: [null],
      remuneratorCtrl: [null],
      datePickerCtrl:null

    });

    this.filteredRemunerators = this.addExpenseForm.get('remuneratorCtrl').valueChanges
      .pipe(
        startWith(''),
        map(remunarator => remunarator ? this._filterRemunerator(remunarator) : this.remunerators)
      );

    this.filteredBenificiary = this.addExpenseForm.get('beneficiaryCtrl').valueChanges
      .pipe(
        startWith(''),
        map(benificiary => benificiary ? this._filterBenificiary(benificiary) : this.benificiaries)
      );
  }

  private _filterBenificiary(value: string): IBeneficiary[] {
    const filterValue = value.toLowerCase();
    return this.benificiaries.filter(benificiary => benificiary.firstName.toLowerCase().indexOf(filterValue) === 0);
  }

  private _filterRemunerator(value: string): IExpense[] {
    const filterValue = value.toLowerCase();
    return this.remunerators.filter(remunarator => remunarator.expenseDescription.toLowerCase().indexOf(filterValue) === 0);
  }


  /*
  onSubmit(addExpenseForm : NgForm){
      console.log(addExpenseForm.valid);
      this.expenseService.postExpense(this.expense).subscribe(
        result=>console.log(result),
        error=>console.log(error)
      );
  }
*/
  /*
  onBlur(expenseDescriptionField:NgModel){
    console.log(expenseDescriptionField.valid);
    
  }
*/

  expense: IExpense = {
    beneficiary: null,
    expenseAmount: null,
    expenseDescription: null,
    expenseId: null,
    benificiaryRating: null,
    expenseType: null,
    transactionDate: null,
    imageUrl: null,
    remunerator: null

  }
}

interface IExpenseType {
  value: string;
  viewValue: string;
}


