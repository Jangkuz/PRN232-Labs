import os
import subprocess
import requests
import time

# Config
PROMETHEUS_URL = os.getenv("PROMETHEUS_URL", "http://localhost:9090")
SERVICE_NAME = os.getenv("SERVICE_NAME", "myapp_web")
MIN_REPLICAS = int(os.getenv("MIN_REPLICAS", 1))
MAX_REPLICAS = int(os.getenv("MAX_REPLICAS", 5))
SCALE_UP_THRESHOLD = float(os.getenv("SCALE_UP_THRESHOLD", 15))  # e.g., CPU > 70%
SCALE_DOWN_THRESHOLD = float(os.getenv("SCALE_DOWN_THRESHOLD", 5))  # CPU < 30%
INTERVAL = int(os.getenv("INTERVAL", 30))  # seconds


def get_cpu_usage():
    # Example PromQL query: average CPU usage %
    query = "avg(rate(container_cpu_usage_seconds_total[1m])) * 100"
    try:
        resp = requests.get(f"{PROMETHEUS_URL}/api/v1/query", params={"query": query})
        result = resp.json()["data"]["result"]
        if result:
            return float(result[0]["value"][1])
    except Exception as e:
        print("Error querying Prometheus:", e)
    return None


def get_current_replicas():
    output = subprocess.check_output(
        ["docker", "service", "ls", "--format", "{{.Name}} {{.Replicas}}"]
    )
    for line in output.decode().splitlines():
        name, replicas = line.split()[0], line.split()[1]
        if name == SERVICE_NAME:
            return int(replicas.split("/")[0])
    return MIN_REPLICAS


def scale_service(count):
    subprocess.call(["docker", "service", "scale", f"{SERVICE_NAME}={count}"])


def main():
    while True:
        cpu = get_cpu_usage()
        if cpu is None:
            print("No metrics available, skipping...")
            time.sleep(INTERVAL)
            continue

        replicas = get_current_replicas()
        print(f"CPU={cpu:.2f}% | Replicas={replicas}")

        if cpu > SCALE_UP_THRESHOLD and replicas < MAX_REPLICAS:
            new_replicas = replicas + 1
            print(f"Scaling UP to {new_replicas}")
            scale_service(new_replicas)
        elif cpu < SCALE_DOWN_THRESHOLD and replicas > MIN_REPLICAS:
            new_replicas = replicas - 1
            print(f"Scaling DOWN to {new_replicas}")
            scale_service(new_replicas)

        time.sleep(INTERVAL)


if __name__ == "__main__":
    main()
