import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { REPL_MODE_STRICT } from 'repl';
import { error } from 'protractor';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
 
 users:any;
  constructor ( private accountService: AccountService){
     
  }
  ngOnInit() {
    this.setCurrentUser();

  }
  setCurrentUser(){

    const user: User = JSON.parse(localStorage.getItem('user') || '{}');
    this.accountService.setCurrentUser(user);

  }


}
