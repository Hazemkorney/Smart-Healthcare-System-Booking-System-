package com.clinic.ms01.rbac;

import java.util.Set;

public record Role(String code, Set<Permission> permissions) {
}
