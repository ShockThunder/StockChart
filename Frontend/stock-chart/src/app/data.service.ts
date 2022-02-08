import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {StockModel} from "./stock-model";
import {Observable} from "rxjs";
import {environment} from "../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private serverUrl = `${environment.API_URL}/stockdata`;
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };
  constructor(private http: HttpClient) {

  }

  getData(): Observable<StockModel[]> {
    return this.http.get<StockModel[]>(this.serverUrl, this.httpOptions)
  }
}
