import http from 'k6/http';
import { check } from 'k6';

const words = ['Family', 'Pizzas', 'Boisson', 'Bread', 'Drinks', 'Chicken', 'Prawn', 'Garlic', 'Chips', 'Pepsi' ]

export default function () {
  const terms = [1, 2, 3].map(() => words[Math.trunc(Math.random() * words.length)]);
  const search = encodeURIComponent(terms.join(' '));
  const res = http.get(`http://localhost:5000/api/1.0/Voucher/GetVouchersByNameSearch?search=${search}`, {
    headers: {
      "X-API-KEY": "034ede0baffa42e4a1186f88ff2b825b"
    }
  });
  check(res, { 'status was 200': (r) => r.status == 200 });
}
