import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { OutputGraphComponent } from './output-graph/output-graph.component';
import { FormsModule } from "@angular/forms";
import { HighchartsChartModule } from "highcharts-angular";


@NgModule({
  declarations: [
    AppComponent,
    OutputGraphComponent
  ],
  imports: [
    BrowserModule,
    HighchartsChartModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
