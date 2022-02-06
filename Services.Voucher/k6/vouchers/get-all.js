import http from 'k6/http';
import { check } from 'k6';

export default function () {
  const count = 1 + Math.trunc(Math.random() * 99)
  const offset = Math.trunc(Math.random() * 1000)
  const res = http.get(`http://localhost:5000/api/1.0/Voucher/Get?count=${count}&offset=${offset}`, {
    headers: {
      "X-API-KEY": "034ede0baffa42e4a1186f88ff2b825b"
    }
  });
  check(res, { 'status was 200': (r) => r.status == 200 });
}
