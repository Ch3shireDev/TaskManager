import requests
import bs4
import re
from datetime import datetime, timedelta


def format_time(time):
    m = re.match(r'(\d+):(\d+)(am|pm)', time)
    hh, mm, period = m.groups()

    hh = int(hh)
    mm = int(mm)
    if period == 'pm' and hh < 12:
        hh += 12

    return f'{hh:02d}:{mm:02d}'


def get_activities(url, params, headers):

    r = requests.get(url=url, params=params, headers=headers)

    html_doc = r.content.decode(encoding='utf-8')

    bs = bs4.BeautifulSoup(html_doc, 'html.parser')

    for li in bs.findAll('li'):
        lines = li.text
        lines = li.text.splitlines()
        lines = [x.strip() for x in lines if x]
        text = ' '.join(lines)
        text = text.split()
        hour, project = text[0], text[-1]
        time = format_time(hour)
        yield time, project


def get_work_data(date_from, date_to, headers):
    url = "https://gitlab.fraktal.com.pl/users/igor.nowicki/calendar_activities"

    date = datetime.fromisoformat(date_from)

    datestr = date.strftime('%Y-%m-%d')

    work_data = []

    while datestr != date_to:
        date += timedelta(days=1)
        datestr = date.strftime('%Y-%m-%d')
        print(datestr)

        params = {'date': datestr,
                  '_': '1626786165413'
                  }

        if date.isoweekday() in [6, 7]:
            continue

        work = [{'hour': x, 'project': y}
                for x, y in get_activities(url, params, headers)]
        work_data.append({'date': datestr, 'work': work})

    return work_data
