// 数据验证器
export default class Validator {
  // 邮箱格式验证
  static validateEmail(email: string): boolean {
    const pattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return pattern.test(email.toLowerCase());
  }

  // 手机号码验证
  static validatePhone(phone: string): boolean {
    const pattern = /^1[34578]\d{9}$/;
    return pattern.test(phone);
  }

  // 网址验证
  static validateUrl(url: string) {
    const pattern = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
    return pattern.test(url.toLowerCase());
  }
}