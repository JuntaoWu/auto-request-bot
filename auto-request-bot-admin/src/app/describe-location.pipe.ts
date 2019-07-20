import { Pipe, PipeTransform } from '@angular/core';
import { LocationModel } from './location-model.interface';

@Pipe({
  name: 'describeLocation'
})
export class DescribeLocationPipe implements PipeTransform {

  transform(value: string, args?: LocationModel[]): any {
    let result = '';
    if (args && args.length) {
      const locationModel = args.find(i => i.value === value);
      result = locationModel && locationModel.text || '';
    }
    return result;
  }

}
