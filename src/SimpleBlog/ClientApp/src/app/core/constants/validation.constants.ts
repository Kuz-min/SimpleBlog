export class ValidationConstants {
  public static readonly USERNAME_REG_EX = /^[a-z]{1}[a-z0-9|\-|_]{2,23}[a-z0-9]{1}$/i;
  public static readonly EMAIL_REG_EX = /^[^@\s]+@[^@\s]+$/i;
}
