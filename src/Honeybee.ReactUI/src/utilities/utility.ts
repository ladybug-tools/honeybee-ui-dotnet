export const GetEnumKeys = <T extends string, TEnumValue extends string | number>(enumVariable: { [key in T]: TEnumValue }): Array<T>=> {
    return Object.keys(enumVariable) as Array<T>;
  }