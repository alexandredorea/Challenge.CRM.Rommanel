export interface ResultError {
  code:    string;
  message: string;
}

export interface Result<T> {
  success: boolean;
  message: string;
  data:    T | null;
  errors:  ResultError[];
}