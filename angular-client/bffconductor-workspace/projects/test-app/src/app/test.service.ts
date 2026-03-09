import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface ErrorRequest {
  errorCode: string | null;
  errorMessage: string | null;
}

@Injectable({ providedIn: 'root' })
export class TestService {
  constructor(private http: HttpClient) {}

  post(request: ErrorRequest): Observable<boolean> {
    return this.http.post<boolean>(`${environment.apiBase}/test/error`, request);
  }

  postControllerLevel(request: ErrorRequest): Observable<boolean> {
    return this.http.post<boolean>(`${environment.apiBase}/attributetest/controller`, request);
  }

  postActionLevel(request: ErrorRequest): Observable<boolean> {
    return this.http.post<boolean>(`${environment.apiBase}/attributetest/action`, request);
  }
}
