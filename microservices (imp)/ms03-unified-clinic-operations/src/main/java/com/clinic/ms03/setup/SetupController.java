package com.clinic.ms03.setup;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/setup")
public class SetupController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-001..US-007 setup domain placeholder.
        return ResponseEntity.ok("Setup module scaffold");
    }
}
