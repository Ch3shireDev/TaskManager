from lib import get_work_data
from datetime import datetime, timedelta
import yaml

headers = {
    'Accept': 'text/html, */*; q=0.01',
    'Accept-Language': 'pl,en-US;q=0.7,en;q=0.3',
    'Accept-Encoding': 'gzip, deflate, br',
    'Referer': 'https://gitlab.fraktal.com.pl/igor.nowicki',
    'X-CSRF-Token': 'YvaiNBKyTebDrfaj/qoCaBzAzgY1vo7sMWG/vxXcPet+PkmmXRpIsnzWpsFjQi8X8rT7x0ewKXw9U2uJAdqxHQ==',
    'X-Requested-With': 'XMLHttpRequest',
    'Connection': 'keep-alive',
    'Cookie': 'sidebar_collapsed=true; diff_view=parallel; remember_user_token=W1s2XSwiJDJhJDEwJE9uMDFqQ0V5THl3ckhCWlUwT2FYZ3UiLCIxNjI1ODMwNDkzLjgzODc5OSJd--90ecb2586d8756b4e4c51d38f152fd997b851f94; _gitlab_session=21f6a1575908734538e476d9959e2b6a; collapsed_gutter=true',
    'TE': 'Trailers'
}

date_from = '2021-01-01'
date_to = datetime.now().date().strftime('%Y-%m-%d')

print(f"From {date_from} to {date_to}")

work_data = get_work_data(date_from, date_to, headers)
fname = 'work.yml'

with open(fname, 'w+', encoding='utf-8') as f:
    yaml.dump(work_data, f, Dumper=yaml.SafeDumper)
    f.close()

print(f'{fname} created')
