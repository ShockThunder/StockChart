export class StockModel {
  dateTime: string;
  value: string;
  constructor(dateTime: string, value: string) {
    this.dateTime = dateTime;
    this.value = value;
  }
}
