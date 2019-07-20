import { Pipe, PipeTransform } from '@angular/core';
import { CheckInStatus } from './member.model';

@Pipe({
  name: 'describeStatus'
})
export class DescribeStatusPipe implements PipeTransform {

  transform(value: CheckInStatus, args?: any): any {
    let result = '';
    switch (value) {
      case CheckInStatus.Activated:
        result = '已激活';
        break;
      case CheckInStatus.InActivated:
        result = '未激活';
        break;
    }
    return result;
  }

}
