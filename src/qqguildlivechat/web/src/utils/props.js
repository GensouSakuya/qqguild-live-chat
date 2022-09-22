import { parse as qsp } from 'query-string';
import { mapValues, pick } from 'lodash';

export const defaultProps = {
  guildName: '',
  room: '',
  connection: null,
  anchor: '',
  display: 'bottom',
  stay: '',
  limit: '',
  giftComb: '',
  giftPin: '',
  delay: '',
  blockUID: '',
};
Object.freeze(defaultProps);

export const intProps = ['anchor', 'stay', 'giftComb', 'limit', 'giftPin', 'delay'];
Object.freeze(intProps);

export const intPropsSet = new Set(intProps);
Object.freeze(intPropsSet);

export const isIntProp = name => intPropsSet.has(name);

const intPropsDefault = {
  faceExpireDay: 7,
};
Object.freeze(intPropsDefault);

export const propsType = mapValues(defaultProps, (v, k) => (intPropsSet.has(k) ? Number : String));
Object.freeze(propsType);

export const selectOptions = {
  display: [
    {
      value: 'bottom',
      text: '自底部',
    },
    {
      value: 'top',
      text: '从顶部',
    },
  ],
};
Object.freeze(selectOptions);

export const parseProps = qs =>
  mapValues(
    pick(
      {
        ...defaultProps,
        ...qsp(qs),
      },
      Object.keys(defaultProps)
    ),
    (v, k) => {
      if (isIntProp(k)) return (v && parseInt(v)) || intPropsDefault[k] || 0;
      if (k in selectOptions) return selectOptions[k].some(({ value }) => value === v) ? v : defaultProps[k];
      return v || defaultProps[k];
    }
  );
