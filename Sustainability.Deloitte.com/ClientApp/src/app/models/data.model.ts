export interface cloudSubscription {
  rgGroups: rgGroup[];
}

export interface rgGroup {
  resourceGroup: string;
  resources: resource[];
}

export interface resource {
  location: string;
  name: string;
  tier: string;
  type: string;
  namespace: string;
  carbonEmission: number;
}
