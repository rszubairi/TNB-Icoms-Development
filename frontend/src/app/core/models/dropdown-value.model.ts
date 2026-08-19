export interface DropdownValue {
  dropdownValueId: number;
  categoryCode: string;
  valueCode: string;
  valueLabel: string;
  parentCode: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface DropdownCategory {
  code: string;
  label: string;
  hasParent: boolean;
}

export interface DropdownCategoriesResponse {
  categories: DropdownCategory[];
  outageTypeParents: string[];
}

export interface CreateDropdownValueRequest {
  categoryCode: string;
  valueLabel: string;
  parentCode: string | null;
}

export interface UpdateDropdownValueRequest {
  valueLabel: string;
  parentCode: string | null;
  isActive: boolean;
}
