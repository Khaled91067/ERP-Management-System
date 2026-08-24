export interface NavItem {
  label: string;
  route: string;
  icon: string;
}

export interface NavGroup {
  name: string;
  icon?: string;
  items: NavItem[];
  hideHeader?: boolean;
}
